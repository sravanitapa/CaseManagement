using System;

// Thrown when a domain rule is broken (an empty title, a bad rating, and so on).
public sealed class DomainException(string message) : Exception(message);

