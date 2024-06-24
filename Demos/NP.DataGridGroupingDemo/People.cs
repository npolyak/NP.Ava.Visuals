using System.Collections.ObjectModel;

namespace NP.DataGridGroupingDemo
{
    public class People : ObservableCollection<Person>
    {
        public void AddPerson(string firstName, string middleName, string lastName)
        {
            this.Add(new Person { FirstName = firstName, MiddleName = middleName, LastName = lastName });    
        }

        public People()
        {
            AddPerson("John", "Lackland", "Plantagenet");
            AddPerson("Richard", "Lionheart", "Plantagenet");
            AddPerson("Richard", "II", "Plantagenet");
            AddPerson("Richard", "III", "Plantagenet");
            AddPerson("Henry", "VII", "Tudor");
            AddPerson("Elizabeth", "I", "Tudor");
        }
    }
}
