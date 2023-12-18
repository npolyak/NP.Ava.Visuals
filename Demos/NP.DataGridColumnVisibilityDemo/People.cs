using System.Collections.ObjectModel;

namespace NP.DataGridColumnVisibilityDemo
{
    public class People : ObservableCollection<Person>
    {
        public void AddPerson(string firstName, string lastName)
        {
            this.Add(new Person { FirstName = firstName, LastName = lastName });    
        }

        public People()
        {
            AddPerson("John", "Plantagenet");
            AddPerson("Richard", "Plantagenet");
            AddPerson("Henry", "Tudor");
            AddPerson("Elizabeth", "Tudor");
        }
    }
}
