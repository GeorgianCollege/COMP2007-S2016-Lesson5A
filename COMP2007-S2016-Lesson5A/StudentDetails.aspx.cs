using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// using statements required for EF DB access
using COMP2007_S2016_Lesson5A.Models;
using System.Web.ModelBinding;

namespace COMP2007_S2016_Lesson5A
{
    public partial class StudentDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetStudent();
            }
        }

        protected void GetStudent()
        {
            // populate the form with existing student record
            int StudentID = Convert.ToInt32(Request.QueryString["StudentID"]);

            // connect the DB with EF
            using (DefaultConnection db = new DefaultConnection())
            {
                // populate a student instance with the StudentID from the URL parameter
                Student updateStudent = (from student in db.Students
                                         where student.StudentID == StudentID
                                         select student).FirstOrDefault();

                // map the student properties to the form controls
                if(updateStudent != null)
                {
                    LastNameTextBox.Text = updateStudent.LastName;
                    FirstNameTextBox.Text = updateStudent.FirstMidName;
                    EnrollmentDateTextBox.Text = updateStudent.EnrollmentDate.ToString("yyyy-MM-dd");
                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            // Redirect back to Students page
            Response.Redirect("~/Students.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Use EF to connect to the server
            using (DefaultConnection db = new DefaultConnection())
            {
                // use the Student model to create a new student object and
                // save a new record
                Student newStudent = new Student();

                // add form data to the new student record
                newStudent.LastName = LastNameTextBox.Text;
                newStudent.FirstMidName = FirstNameTextBox.Text;
                newStudent.EnrollmentDate = Convert.ToDateTime(EnrollmentDateTextBox.Text);

                // use LINQ to ADO.NET to add / insert new student into the database
                db.Students.Add(newStudent);

                // save our changes
                db.SaveChanges();

                // Redirect back to the updated students page
                Response.Redirect("~/Students.aspx");
            }
        }
    }
}