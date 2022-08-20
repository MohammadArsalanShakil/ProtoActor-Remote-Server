using Avalonia.Controls;
using System.Threading.Tasks;
using Proto;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using static Proto.Remote.GrpcNet.GrpcNetRemoteConfig;
using chat.messages;
using System.Collections.Generic;
using ProtoActor_Remote_Server.Common;

namespace ProtoActor_Remote_Server
{
    public partial class MainWindow : Window
    {
        private RootContext context;

        public MainWindow()
        {
            InitializeComponent();
            InitializeActorSystem();
            SpawnServer();
        }

        ~MainWindow()
        {
            context.System.Remote().ShutdownAsync().GetAwaiter().GetResult();
        }

        private void InitializeActorSystem()
        {
            var system = new ActorSystem()
                .WithRemote(BindToAllInterfaces(Globals.ownIP, Globals.ownPort)
                .WithProtoMessages(ChatReflection.Descriptor)
                .WithRemoteDiagnostics(true));
            system.Remote().StartAsync();

            context = system.Root;
        }

        private void SpawnServer()
        {
            var clients = new HashSet<PID>();

            context.SpawnNamed(
                Props.FromFunc(
                    ctx =>
                    {
                        switch (ctx.Message)
                        {
                            case Connected connected:
                                break;
                            case Connect connect:
                                if (!clients.Contains(connect.Sender))
                                    clients.Add(connect.Sender);

                                ctx.Send(connect.Sender, new Connected { Message = "Welcome!" });
                                break;
                            case SayRequest sayRequest:
                                foreach (var client in clients)
                                {
                                    ctx.Send(client, new SayResponse { UserName = sayRequest.UserName, Message = sayRequest.Message });
                                }
                                break;
                            default:
                                break;
                        }

                        return Task.CompletedTask;
                    }
                ),
                  Globals.ownActorName
            );
        }
    }
}

