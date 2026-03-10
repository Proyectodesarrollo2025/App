using System;
using System.Text.Json;

var json = "{\"id\": \"Q60\", \"num\": 123}";
using var doc = JsonDocument.Parse(json);
var id = doc.RootElement.GetProperty("id").ToString();
var num = doc.RootElement.GetProperty("num").ToString();

Console.WriteLine($"ID: |{id}|");
Console.WriteLine($"NUM: |{num}|");
