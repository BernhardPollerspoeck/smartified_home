﻿using System.ComponentModel.DataAnnotations;

namespace smart.database;

public class ElementHandler
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public EElementType ElementType { get; set; } = default!;


    public string? SettingsData { get; set; } = default!;

    public bool Enabled { get; set; }

    public bool ProcessRunning { get; set; }
    public bool SignalConnected { get; set; }


}