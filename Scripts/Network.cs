using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
    // network
    public bool Active = false;

    // server
    List<SnapShot> ClientSnapShots = new List<SnapShot>();
    List<int> ConnectedClients = new List<int>();


    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if (Active)
        {
            // check if there is new data
            object[] packet = null;
            // send data to each client
            // we may want to just send all game data to all clients to reduce complexity in early days
            foreach (int clientID in ConnectedClients)
            {
                // for each client, send them update of every other client
                if (ConnectedClients.Count > 1)
                {
                    foreach (int otherClientID in ConnectedClients)
                    {
                        if (clientID != otherClientID)
                        {
                            packet = GetPacket(clientID, otherClientID);

                            if (packet != null)
                            {
                                RpcUnreliableId(clientID, "ReceivePacket", packet);
                            }
                        }
                    }
                }
                else
                {
                    // send acknowledgement packet so single player doesn't keep huge history
                    packet = GetPacket(clientID, 0);
                    if (packet != null)
                    {
                        RpcUnreliableId(clientID, "ReceivePacket", packet);
                    }
                }
            }
        }
    }

    private object[] GetPacket(int clientID, int otherClientID)
    {
        int packetNum = 0;
        Vector3 trans = new Vector3();
        // check if other clients have sent updated data
        // check if there are changes to projectiles or other ents
        packetNum = ClientSnapShots.FindLast(ss => ss.ClientID == clientID).PacketNumber;
        if (otherClientID != 0)
        {
            trans = ClientSnapShots.FindLast(ss => ss.ClientID == otherClientID).Translation;
        }

        object[] packet = { packetNum, clientID, otherClientID, trans };
        return packet;
    }

    public void AddClient(int id)
    {
        GD.Print("adding client " + id);
        ConnectedClients.Add(id);
        SnapShot ss = new SnapShot(0, id, new Vector3());
        ClientSnapShots.Add(ss);
    }

    [Remote]
    public void ReceivePacket(int packetNum, int clientID, int otherClientID, Vector3 trans)
    {
        // if server receives packet, put changes in, update response packet with packetnumber
        SnapShot lastPacket = ClientSnapShots.FindLast(ss => ss.ClientID == clientID);
        if (packetNum > lastPacket.PacketNumber)
        {
            // find player, if not exist create at translation
            Main main = (Main)GetNode("/root/OpenFortress/Main");
            if (!main.HasNode(clientID.ToString()))
            {
                main.AddPlayer(false, clientID);

                // propagate to all clients
                throw new NotImplementedException();
            }

            if (trans != lastPacket.Translation)
            {
                MovePlayer(clientID, trans);                   
            }             

            // add new snapshot
            if (ClientSnapShots.FindAll(ss => ss.ClientID == clientID).Count > 32)
            {
                ClientSnapShots.Remove(ClientSnapShots.Find(ss => ss.ClientID == clientID));
            }
            ClientSnapShots.Add(new SnapShot(packetNum, clientID, trans));
        }
    }

// receiving packets
    private void MovePlayer(int clientID, Vector3 trans)
    {
        Player p = (Player)GetNode("/root/OpenFortress/Main/" + clientID.ToString());
        p.Translation = trans;
        GD.Print (p.Translation);
    }
}

public class SnapShot
{
    public int PacketNumber;
    public int ClientID;
    public Vector3 Translation;
    public SnapShot(int packetNum, int clientID, Vector3 trans)
    {
        this.PacketNumber = packetNum;
        this.ClientID = clientID;
        this.Translation = trans;
    }
}
// packet
    // player join, quit etc commands
    // player chat
    // shoot
    // move

    // packet represents player state
