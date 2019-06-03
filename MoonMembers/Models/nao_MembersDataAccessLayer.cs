using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace MoonMembers.Models
{
    public class MembersDataAccessLayer
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MoonMembers"].ConnectionString;

        /*
         * To View all employees details
         */
        public IEnumerable<Members> GetAllMembers(int? status)
        {
            List<Members> lstMembers = new List<Members>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllMembers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (status != null)
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                }
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    Members member = new Members();

                    member.memberId = Convert.ToInt32(rdr["memberId"]);
                    member.memberName = rdr["memberName"].ToString();
                    member.memberEmail = rdr["memberEmail"].ToString();
                    member.memberBirthdate = (DateTime) rdr["memberBirthdate"];
                    member.memberPhoto = rdr["memberPhoto"].ToString();
                    member.memberOrder = (int) rdr["memberOrder"];
                    member.memberStatus = (byte) rdr["memberStatus"];
                }

                con.Close();
            }

            return lstMembers;
        }

        /*
         * To Add new member record
         */
        public void AddMember(Members member)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spAddMember", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MemberName", member.memberName);
                cmd.Parameters.AddWithValue("@MemberEmail", member.memberEmail);
                cmd.Parameters.AddWithValue("@MemberPhoto", member.memberPhoto);
                cmd.Parameters.AddWithValue("@MemberBirthdate", member.memberBirthdate);
                cmd.Parameters.AddWithValue("@MemberOrder", member.memberOrder);
                cmd.Parameters.AddWithValue("@MemberStatus", member.memberStatus);
                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }
        }

        /*
         * To Update the records of a particular member
         */
        public void UpdateMember(Members member)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateMember", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MemberId", member.memberId);
                cmd.Parameters.AddWithValue("@MemberName", member.memberName);
                cmd.Parameters.AddWithValue("@MemberEmail", member.memberEmail);
                cmd.Parameters.AddWithValue("@MemberPhoto", member.memberPhoto);
                cmd.Parameters.AddWithValue("@MemberBirthdate", member.memberBirthdate);
                cmd.Parameters.AddWithValue("@MemberOrder", member.memberOrder);
                cmd.Parameters.AddWithValue("@MemberStatus", member.memberStatus);
                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }
        }

        /*
         * Get the details of a particular member
         */
        public Members GetMemberData(int? id)
        {
            Members member = new Members();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sqlQuery = "";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    member.memberId = Convert.ToInt32(rdr["memberId"]);
                    member.memberName = rdr["memberName"].ToString();
                    member.memberEmail = rdr["memberEmail"].ToString();
                    member.memberPhoto = rdr["memberPhoto"].ToString();
                    member.memberOrder = (int)rdr["memberOrder"];
                    member.memberStatus = (byte)rdr["memberStatus"];
                }
            }

            return member;
        }

        /*
         * To Delete the record of a particular member
         */
        public void DeleteMember(int? id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteMember", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MemberId", id);
                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }
        }
    }
}