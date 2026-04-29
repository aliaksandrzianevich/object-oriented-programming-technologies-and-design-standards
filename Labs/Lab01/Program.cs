namespace Lab01;

// Interface Корабль
internal interface IKorabl
{
    string GetName();
    string GetType();
    void DisplayInfo();
}

// Abstract class Военный Корабль
internal abstract class VoennyKorabl : IKorabl
{
    protected string name;
    protected string type;
    protected int crew;
    protected double displacement;
    protected int weaponCount;
    protected double speed;

    public VoennyKorabl(string name, string type, int crew, double displacement, int weaponCount, double speed)
    {
        this.name = name;
        this.type = type;
        this.crew = crew;
        this.displacement = displacement;
        this.weaponCount = weaponCount;
        this.speed = speed;
    }

    public VoennyKorabl(string name, int crew)
    {
        this.name = name;
        this.crew = crew;
        this.type = "Военный корабль";
        this.displacement = 5000;
        this.weaponCount = 10;
        this.speed = 20;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public string Type
    {
        get => type;
        set => type = value;
    }

    public int Crew
    {
        get => crew;
        set => crew = value;
    }

    public double Displacement
    {
        get => displacement;
        set => displacement = value;
    }

    public int WeaponCount
    {
        get => weaponCount;
        set { weaponCount = value; }
    }

    public double Speed
    {
        get => speed;
        set { speed = value; }
    }

    public string GetName() => name;

    public string GetType() => type;

    public abstract void DisplayInfo();
    public abstract void OpenFire();
    public abstract void LaunchAircraft();

    public override string ToString()
        => $"Корабль: {name}, Тип: {type}, Экипаж: {crew}, Водоизмещение: {displacement} тонн, " +
           $"Вооружение: {weaponCount}, Скорость: {speed} узлов";

    public static bool operator ==(VoennyKorabl k1, VoennyKorabl k2)
    {
        if (ReferenceEquals(k1, k2)) return true;
        if (ReferenceEquals(k1, null) || ReferenceEquals(k2, null)) return false;
        return k1.displacement == k2.displacement && k1.crew == k2.crew;
    }

    public static bool operator !=(VoennyKorabl k1, VoennyKorabl k2) => !(k1 == k2);

    public static bool operator >(VoennyKorabl k1, VoennyKorabl k2) => k1.displacement > k2.displacement;

    public static bool operator <(VoennyKorabl k1, VoennyKorabl k2) => k1.displacement < k2.displacement;

    public override bool Equals(object obj)
    {
        if (obj is VoennyKorabl k)
            return this == k;
        return false;
    }

    public override int GetHashCode() => (name + displacement).GetHashCode();
}

// Class Авианосец
internal class Avianosec : VoennyKorabl
{
    private int _aircraftCount;
    private double _hangars;
    private bool _catapultSystem;

    public Avianosec(string name,
        int crew,
        double displacement,
        int weaponCount,
        double speed,
        int aircraftCount,
        double hangars,
        bool catapultSystem) : base(name, "Авианосец", crew, displacement, weaponCount, speed)
    {
        _aircraftCount = aircraftCount;
        _hangars = hangars;
        _catapultSystem = catapultSystem;
    }

    public Avianosec(string name, int aircraftCount) : base(name, 2000)
    {
        type = "Авианосец";
        displacement = 80000;
        _aircraftCount = aircraftCount;
        _hangars = 5;
        _catapultSystem = true;
    }

    public int AircraftCount
    {
        get => _aircraftCount;
        set => _aircraftCount = value;
    }

    public double Hangars
    {
        get => _hangars;
        set => _hangars = value;
    }

    public bool CatapultSystem
    {
        get => _catapultSystem;
        set => _catapultSystem = value;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"=== Информация об авианосце ===");
        Console.WriteLine($"Название: {name}");
        Console.WriteLine($"Тип: {type}");
        Console.WriteLine($"Экипаж: {crew} человек");
        Console.WriteLine($"Водоизмещение: {displacement} тонн");
        Console.WriteLine($"Вооружение: {weaponCount} орудий");
        Console.WriteLine($"Скорость: {speed} узлов");
        Console.WriteLine($"Количество самолетов: {_aircraftCount}");
        Console.WriteLine($"Ангары: {_hangars}");
        Console.WriteLine($"Система катапультирования: {(_catapultSystem ? "Включена" : "Отключена")}");
    }

    public override void OpenFire()
    {
        Console.WriteLine($"{name} открывает огонь! {weaponCount} орудий в действии!");
    }

    public override void LaunchAircraft()
    {
        Console.WriteLine($"{name} запускает авиагруппу из {_aircraftCount} самолетов!");
        if (_catapultSystem)
            Console.WriteLine("Система катапультирования активирована!");
    }

    public override string ToString()
        => base.ToString() + $", Самолеты: {_aircraftCount}, Ангары: {_hangars}, " +
           $"Катапульта: {(_catapultSystem ? "Да" : "Нет")}";

    public static Avianosec operator ++(Avianosec a)
    {
        a._aircraftCount++;
        return a;
    }

    public static Avianosec operator --(Avianosec a)
    {
        if (a._aircraftCount > 0)
            a._aircraftCount--;
        return a;
    }

    public static bool operator >=(Avianosec a1, Avianosec a2) => a1._aircraftCount >= a2._aircraftCount;

    public static bool operator <=(Avianosec a1, Avianosec a2) => a1._aircraftCount <= a2._aircraftCount;
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Создание объектов авианосцев
        var avianosec1 = new Avianosec("USS Nimitz", 2500, 100000, 150, 34, 65, 6, true);
        var avianosec2 = new Avianosec("HMS Queen Elizabeth", 75);

        // Вывод информации
        avianosec1.DisplayInfo();
        Console.WriteLine();
        Console.WriteLine(avianosec1.ToString());
        Console.WriteLine();

        avianosec2.DisplayInfo();
        Console.WriteLine();
        Console.WriteLine(avianosec2.ToString());
        Console.WriteLine();

        // Тестирование методов
        avianosec1.OpenFire();
        avianosec1.LaunchAircraft();
        Console.WriteLine();

        // Тестирование операторов сравнения
        Console.WriteLine($"USS Nimitz больше чем HMS Queen Elizabeth: {avianosec1 > avianosec2}");
        Console.WriteLine($"USS Nimitz == HMS Queen Elizabeth: {avianosec1 == avianosec2}");
        Console.WriteLine();

        // Тестирование операторов инкремента/декремента
        Console.WriteLine($"Самолетов на HMS Queen Elizabeth: {avianosec2.AircraftCount}");
        avianosec2++;
        Console.WriteLine($"После инкремента: {avianosec2.AircraftCount}");
        avianosec2--;
        Console.WriteLine($"После декремента: {avianosec2.AircraftCount}");
        Console.WriteLine();

        // Сравнение авианосцев
        if (avianosec1 >= avianosec2)
            Console.WriteLine("HMS Queen Elizabeth имеет не больше самолетов, чем USS Nimitz");
    }
}