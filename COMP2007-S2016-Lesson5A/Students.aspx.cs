using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// using statements that are required to connect to EF DB
using COMP2007_S2016_Lesson5A.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;

namespace COMP2007_S2016_Lesson5A
{
    public partial class Students : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if loading the page for the first time, populate the student grid
            if (!IsPostBack)
            {
                // create two session variables to hold our sorting data
                Session["SortColumn"] = "StudentID";
                Session["SortDirection"] = "ASC";

                // Get the student data
                this.GetStudents();
            }
        }

        /**
         * <summary>
         * This method gets the student data from the DB
         * </summary>
         * 
         * @method GetStudents
         * @returns {void}
         */
        protected void GetStudents()
        {
            // connect to EF
            using (DefaultConnection db = new DefaultConnection())
            {
                // create a query string to add to the LINQ Query
                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();

                // query the Students Table using EF and LINQ
                var Students = (from allStudents in db.Students
                                select new {allStudents.StudentID, allStudents.LastName,
                                    allStudents.FirstMidName, allStudents.EnrollmentDate });

                // bind the result to the GridView
                StudentsGridView.DataSource = Students.AsQueryable().OrderBy(SortString).ToList();
                StudentsGridView.DataBind();
            }
        }

        /**
         * <summary>
         * This method is used to delete Student records from the db using ef
         * </summary>
         * 
         * @method StudentsGridView_RowDeleting
         * @param {object} sender
         * @param {GridViewDeleteEventArgs} e
         * @return {void}
         */
        protected void StudentsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // store which row was selected for deletion
            int selectedRow = e.RowIndex;

            // get the selected StudentID using the grid's Datakey collection
            int StudentID = Convert.ToInt32(StudentsGridView.DataKeys[selectedRow].Values["StudentID"]);

            // use EF to find the selected student from DB and Remove it
            using (DefaultConnection db = new DefaultConnection())
            {
                Student deletedStudent = (from studentRecords in db.Students
                                          where studentRecords.StudentID == StudentID
                                          select studentRecords).FirstOrDefault();

                // remove the student record from the database
                db.Students.Remove(deletedStudent);

                // save changes to the db
                db.SaveChanges();

                // refresh the grid
                this.GetStudents();
            }
        }

        /**
         * <summary>
         * This method enables paging on our StudentsGridViw
         * </summary>
         * 
         * @method StudentsGridView_PageIndexChanging
         * @param {object} sender
         * @param {GridViewPageEventArgs} e
         * @returns {void}
         */
        protected void StudentsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Set the new page number
            StudentsGridView.PageIndex = e.NewPageIndex;

            // refresh the grid
            this.GetStudents();
        }

        protected void StudentsDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // set new Page size
            StudentsGridView.PageSize = Convert.ToInt32(StudentsDropDownList.SelectedValue);

            // refresh the grid
            this.GetStudents();
        }

        protected void StudentsGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            // get the column to sort by
            Session["SortColumn"] = e.SortExpression;

            // refresh the grid
            this.GetStudents();

            //toggle the sort direction
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
        }

        protected void StudentsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if(e.Row.RowType == DataControlRowType.Header) // if the row clicked is the header row
                {
                    LinkButton linkButton = new LinkButton();

                    for (int index = 0; index < StudentsGridView.Columns.Count; index++) // check each column for a click
                    {
                        if(StudentsGridView.Columns[index].SortExpression == Session["SortColumn"].ToString())
                        {
                            if(Session["SortDirection"].ToString() == "ASC")
                            {
                                linkButton.Text = " <i class='fa fa-caret-up fa-lg'></i>";
                            }
                            else
                            {
                                linkButton.Text = " <i class='fa fa-caret-down fa-lg'></i>";
                            }

                            e.Row.Cells[index].Controls.Add(linkButton); // add the new linkbutton to header cell
                        }
                    }
                }
            }
        }
    }
}