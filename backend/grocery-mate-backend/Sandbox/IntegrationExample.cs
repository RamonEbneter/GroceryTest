namespace grocery_mate_backend.Sandbox;

public class IntegrationExample : IPersonProcessor
{
    ISqliteDataAccess _database;

    public IntegrationExample(ISqliteDataAccess database)
    {
        _database = database;
    }

    public PersonModel CreatePerson(string firstName, string lastName, string heightText)
    {
        PersonModel output = new PersonModel();

        if (ValidateName(firstName) == true)
        {
            output.FirstName = firstName;
        }
        else
        {
            throw new ArgumentException("The value was not valid", "firstName");
        }

        if (ValidateName(lastName) == true)
        {
            output.LastName = lastName;
        }
        else
        {
            throw new ArgumentException("The value was not valid", "lastName");
        }

        var height = ConvertHeightTextToInches(heightText);

        if (height.isValid == true)
        {
            output.HeightInInches = height.heightInInches;
        }
        else
        {
            throw new ArgumentException("The value was not valid", "heightText");
        }

        return output;
    }

    public List<PersonModel> LoadPeople()
    {
        string sql = "select * from Person";

        var output = _database.LoadData<PersonModel>(sql);

        return output;
    }

    public void SavePerson(PersonModel person)
    {
        string sql = "insert into Person (FirstName, LastName, HeightInInches) " +
                     "values (@FirstName, @LastName, @HeightInInches)";

        sql = sql.Replace("@FirstName", $"'{person.FirstName}'");
        sql = sql.Replace("@LastName", $"'{person.LastName}'");
        sql = sql.Replace("@HeightInInches", $"{person.HeightInInches}");

        _database.SaveData(person, sql);
    }

    public void UpdatePerson(PersonModel person)
    {
        string sql = "update Person set FirstName = @FirstName, LastName = @LastName" +
                     ", HeightInInches = @HeightInInches where Id = @Id";

        _database.UpdateData(person, sql);
    }

    public (bool isValid, double heightInInches) ConvertHeightTextToInches(string heightText)
    {
        bool isValid = true;
        double heightInInches = 0;

        int feetMarkerLocation = heightText.IndexOf('\'');
        int inchesMarkerLocation = heightText.IndexOf('"');

        if (feetMarkerLocation < 0
            || inchesMarkerLocation < 0
            || inchesMarkerLocation < feetMarkerLocation)
        {
            return (false, 0);
        }

        // Split on both the feet and inches indicators
        string[] heightParts = heightText.Split(new char[] {'\'', '"'});


        // Part 0 should be feet, part 1 should be inches
        if (int.TryParse(heightParts[0], out int feet) == false
            || double.TryParse(heightParts[1], out double inches) == false)
        {
            return (false, 0);
        }

        heightInInches = (feet * 12) + inches;

        return (isValid, heightInInches);
    }

    private bool ValidateName(string name)
    {
        bool output = true;
        char[] invalidCharacters = "`~!@#$%^&*()_+=0123456789<>,.?/\\|{}[]'\"".ToCharArray();

        if (name.Length < 2)
        {
            output = false;
        }

        if (name.IndexOfAny(invalidCharacters) >= 0)
        {
            output = false;
        }

        return output;
    }
}

public interface IPersonProcessor
{
    (bool isValid, double heightInInches) ConvertHeightTextToInches(string heightText);
    PersonModel CreatePerson(string firstName, string lastName, string heightText);
    List<PersonModel> LoadPeople();
    void SavePerson(PersonModel person);
    void UpdatePerson(PersonModel person);
}

public class PersonModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public double HeightInInches { get; set; }

    public string FullName
    {
        get { return $"{FirstName} {LastName}"; }
    }
}

public interface ISqliteDataAccess
{
    List<T> LoadData<T>(string sql);
    void SaveData<T>(T person, string sql);
    void UpdateData<T>(T person, string sql);
}