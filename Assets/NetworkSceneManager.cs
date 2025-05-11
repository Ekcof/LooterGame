using Mirror;
using UnityEngine;
using Zenject;

public class NetworkSceneManager : NetworkManager
{
    [Inject] DiContainer _container;

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		Transform startPos = GetStartPosition();

		var playerGO = _container.InstantiatePrefab(
			playerPrefab,
			startPos
		);

		playerGO.transform.position = startPos != null ? startPos.position : Vector3.zero;
		playerGO.transform.rotation = startPos != null ? startPos.rotation : Quaternion.identity;

		playerGO.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
		NetworkServer.AddPlayerForConnection(conn, playerGO);
	}
}

public static class CustomSerialization
{
    // Extension-����� ��� ������������
    public static void WriteItemBase(this NetworkWriter writer, ItemBase item)
    {
        // ������: ����������� ������� �������
        bool hasItem = item != null;
        writer.WriteBool(hasItem);
        if (!hasItem) return;

        //// ����������, ��������, ��� �������, ����� �����, ��� ��� ���������������
        //if (item is Weapon)
        //{
        //    writer.WriteByte(1);
        //    writer.Write(item as Weapon); // ��������������, ��� ��� Weapon ��� ��������� Write
        //}
        //else if (item is Armor)
        //{
        //    writer.WriteByte(2);
        //    writer.Write(item as Armor); // ���������� ��� Armor
        //}
        //// �������� ��������� ������ ����������� �� ���� �������������
        //else
        //{
        //    throw new System.Exception("Unsupported ItemBase type");
        //}
    }

    // Extension-����� ��� ��������������
    public static ItemBase ReadItemBase(this NetworkReader reader)
    {
        bool hasItem = reader.ReadBool();
        if (!hasItem) return null;

        byte typeId = reader.ReadByte();
        switch (typeId)
        {
            case 1:
                return reader.Read<HealingPotion>(); // ��������������, ��� ��� Weapon ��� ��������� Read
            case 2:
                return reader.Read<StaminaPotion>();
            // �������� ������ ���� ��� �������������
            default:
                throw new System.Exception("Unsupported ItemBase type");
        }
    }
}
