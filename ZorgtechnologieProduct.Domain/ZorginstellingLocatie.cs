using System;
using System.Collections.Generic;

public class Zorginstellingslocatie
{
    public Guid Id { get; set; }
    public required string Naam { get; set; }
    public required string Adres { get; set; }
    public required string Postcode { get; set; }
    public required string Plaats { get; set; }
}
