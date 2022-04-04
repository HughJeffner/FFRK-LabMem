using System;
using System.Threading;
using System.Threading.Tasks;
using FFRK_Machines.Services;
using FFRK_Machines.Services.Adb;
using FFRK_Machines.Services.Notifications;
using Newtonsoft.Json.Linq;
using Stateless;

namespace FFRK_Machines.Machines
{
    /// <summary>
    /// Abstract class that represents a machine
    /// </summary>
    /// <typeparam name="S">An enum of all the possible machine states</typeparam>
    /// <typeparam name="T">An enum of all the available machine triggers</typeparam>
    /// <typeparam name="C">A class containing configuration information for the machine, must inherit from MachineConfiguration</typeparam>
    public abstract class Machine<S, T, C> : Proxy.IProxyMachine
        where C : MachineConfiguration
    {

        // Event Handling
        public event EventHandler<Exception> MachineError;
        public event EventHandler MachineFinished;

        // Local vars and properties
        public Adb Adb { get; set; }
        public StateMachine<S, T> StateMachine { get; set; }
        public C Config { get; set; }
        protected Random rng = new Random();
        public CancellationToken CancellationToken { get; set; }
        private CancellationTokenSource cancelSource = new CancellationTokenSource();

        // Data property
        private JObject mData = null;

        /// <summary>
        /// The current data this state machine is operating on
        /// </summary>
        public JObject Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
                if (value != null) OnDataChanged(value);
            }
        }

        // Abstract Methods
        /// <summary>
        /// Registers this machine with the proxy
        /// </summary>
        /// <param name="Proxy">The proxy to register to</param>
        public abstract void RegisterWithProxy(Proxy Proxy);

        public async Task InterruptTasks()
        {
            cancelSource.CancelAfter(0);
            await Task.Delay(10);
            cancelSource = new CancellationTokenSource();
            CancellationToken = cancelSource.Token;
        }

        /// <summary>
        /// Configures and enables the state machine
        /// </summary>
        /// <returns></returns>
        public Task Enable()
        {
            cancelSource = new CancellationTokenSource();
            CancellationToken = cancelSource.Token;
            ConfigureStateMachine();
            OnEnabled();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles any tasks needed if the controller enables this machine.  Implementors of this class can override this method.
        /// </summary>
        protected virtual void OnEnabled() {}

        /// <summary>
        /// Disables this machine
        /// </summary>
        /// <returns></returns>
        public Task Disable()
        {
            cancelSource.CancelAfter(0);
            OnDisabled();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles any tasks needed if the controller disables this machine.  Does nothing by default, implementors of this class should override this method.
        /// </summary>
        protected virtual void OnDisabled() {}

        /// <summary>
        /// Configures the internal state machine.  Implementors should override this method
        /// </summary>
        public virtual void ConfigureStateMachine()
        {

            if (StateMachine == null) return;

            // Invalid state handling
            StateMachine.OnUnhandledTrigger((state, trigger) =>
            {
                OnMachineError(new InvalidStateException<T,S>(trigger, state));
            });

            // Console output
            if (Config == null) return;
            StateMachine.OnTransitioned((state) => { ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Entering state: {0}", state.Destination); });

        }

        /// <summary>
        /// Called every time the Data propery is set
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnDataChanged(JObject data) {}

        /// <summary>
        /// Called when a state machine error occurs
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMachineError(Exception e)
        {
            // Safely raise the event for all subscribers
            if (MachineError != null) MachineError.Invoke(this, e);
        }

        /// <summary>
        /// Called when the state machine reaches its end state
        /// </summary>
        protected virtual void OnMachineFinished()
        {
            // Safely raise the event for all subscribers
            if (MachineFinished != null) MachineFinished.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Notifies the user, by default embedded wav file
        /// </summary>
        /// <returns></returns>
        public virtual async Task Notify(Notifications.EventType eventType, string message)
        {
            await Notifications.Default.ProcessEvent(eventType, message);
        }

    }
}
