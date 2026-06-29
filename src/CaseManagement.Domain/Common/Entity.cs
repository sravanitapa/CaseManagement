using System;


public abstract class Entity()
{
    //Guid.CreateVersion7() generates sequential GUIDs, which improve PostgreSQL index performance compared to random GUIDs from Guid.NewGuid().
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
}
