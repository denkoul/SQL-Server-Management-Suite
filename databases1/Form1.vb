Imports System.Data.SqlClient
Public Class Form1

    Dim command As New SqlCommand
    Dim data_reader As SqlDataReader
    Dim connection_string As String
    Dim con As New SqlConnection()
    Dim local As Boolean = False
    Dim output As String

    Dim local_filename As String
    Dim c As Int32

    Private Sub Label2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs)

    End Sub


    Sub reseed(ByVal ConnectionString As String, ByVal TableName As String)
        Dim con1 As New System.Data.SqlClient.SqlConnection()
        con1.ConnectionString = ConnectionString
        con1.Open()
        Dim parents As New List(Of String)()
        Try
            Dim delete_fk As New System.Data.SqlClient.SqlCommand("select object_name(parent_object_id)
            from sys.foreign_keys
            WHERE object_name(referenced_object_id) ='" & TableName & "'", con1)
            Try
                Dim dr = delete_fk.ExecuteReader()
                While dr.Read()
                    parents.Add(dr(0).ToString())
                End While
                dr.Close()
            Catch ex As System.Data.SqlClient.SqlException
                MessageBox.Show("There was an error accessing your data. DETAIL: " & ex.ToString())
            End Try
            For Each table As String In parents
                Dim remove_fk As New System.Data.SqlClient.SqlCommand("ALTER TABLE " & table.ToString & " NOCHECK CONSTRAINT ALL", con1)
                remove_fk.ExecuteNonQuery()
            Next
            Dim copy_back As String = "insert into " + TableName + " select * from #temp1"
            Dim first_copy As New System.Data.SqlClient.SqlCommand("Select * into #temp1 from " + TableName, con1)
            Dim delete_Table As New System.Data.SqlClient.SqlCommand("delete from " + TableName, con1)
            Dim second_copy As New System.Data.SqlClient.SqlCommand(copy_back, con1)
            first_copy.ExecuteNonQuery()
            delete_Table.ExecuteNonQuery()
            second_copy.ExecuteNonQuery()
            For Each table As String In parents
                Dim regain_fk As New SqlCommand("ALTER TABLE " & table.ToString & " WITH CHECK CHECK CONSTRAINT ALL", con1)
                regain_fk.ExecuteNonQuery()
            Next
        Catch ex As SqlException
            MessageBox.Show("Can Not execute command", "Error", MessageBoxButtons.OK)
        End Try
        con1.Close()
    End Sub








    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ListBox1.Items.Clear()
        con.Open()
        command.CommandText = TextBox1.Text
        Try
            data_reader = command.ExecuteReader()
            If data_reader.HasRows Then
                While (data_reader.Read())

                    c = 0
                    output = " "
                    While c < data_reader.FieldCount

                        output = output + " " + Convert.ToString(data_reader.GetSqlValue(c))
                        c = c + 1
                    End While


                    ListBox1.Items.Add(output)
                End While
            End If
        Catch ex As SqlException
            MessageBox.Show("Invalid syntax", "Error", MessageBoxButtons.OK)
        End Try

        con.Close()

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button2.Enabled = False

    End Sub

    Private Sub Connect_Click(sender As Object, e As EventArgs) Handles Connect.Click

        con.ConnectionString = "Data Source=" + ip.Text + ";User ID=" + username.Text + ";Password=" + Password.Text








        command.Connection = con



        Try

            con.Open()

            MessageBox.Show("Connection successful", "Connected", MessageBoxButtons.OK)
            Button2.Enabled = True
            Dim select_data
            command.CommandText = "Select name FROM master.dbo.sysdatabases"

            data_reader = command.ExecuteReader()
            While data_reader.Read
                ComboBox1.Items.Add(data_reader("name").ToString())
            End While



            con.Close()
        Catch ex As SqlException
            MessageBox.Show("Wrong login details.", "Error", MessageBoxButtons.OK)
        End Try





    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim table_to_resed As String = TextBox2.Text
        con.Open()
        Dim parents As New List(Of String)()

        Try

            Dim delete_fk As New SqlCommand("select object_name(parent_object_id)
            from sys.foreign_keys
            WHERE object_name(referenced_object_id) ='" & table_to_resed & "'", con)

            Try
                Dim dr = delete_fk.ExecuteReader()
                While dr.Read()
                    parents.Add(dr(0).ToString())
                End While
                dr.Close()
            Catch ex As SqlException
                ' Do some logging or something. 
                MessageBox.Show("There was an error accessing your data. DETAIL: " & ex.ToString())
            End Try

            For Each table As String In parents
                Dim remove_fk As New SqlCommand("ALTER TABLE " & table.ToString & " NOCHECK CONSTRAINT ALL", con)
                remove_fk.ExecuteNonQuery()
            Next

            Dim copy_back As String = "insert into " + table_to_resed + " select * from #temp1  "


            Dim first_copy As New SqlCommand("Select * into #temp1 from " + table_to_resed, con)
            Dim delete_Table As New SqlCommand("delete from " + table_to_resed, con)
            Dim second_copy As New SqlCommand(copy_back, con)


            first_copy.ExecuteNonQuery()
            delete_Table.ExecuteNonQuery()
            second_copy.ExecuteNonQuery()

            For Each table As String In parents
                Dim regain_fk As New SqlCommand("ALTER TABLE " & table.ToString & " WITH CHECK CHECK CONSTRAINT ALL", con)
                regain_fk.ExecuteNonQuery()
            Next

        Catch ex As SqlException
            MessageBox.Show("Can Not execute command", "Error", MessageBoxButtons.OK)
        End Try

        con.Close()




    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged



        Try

            con.ConnectionString = "Data Source=" & ip.Text + ";Initial Catalog=" + ComboBox1.SelectedItem.ToString + ";User ID=" + username.Text + ";Password=" + Password.Text


            command.Connection = con
            Try
                con.Open()

                MessageBox.Show("Database changed To " & ComboBox1.SelectedItem.ToString, "Connected", MessageBoxButtons.OK)
                Button2.Enabled = True
                Button3.Text = "List " & ComboBox1.SelectedItem.ToString & "'s tables"







                con.Close()
            Catch ex As SqlException
                MessageBox.Show("Wrong login details", "Error", MessageBoxButtons.OK)
            End Try


        Catch ex As Exception
            MsgBox("An error occurced. Try again")

        End Try

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox1.Text = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'"
        Me.Button2.PerformClick()
        TextBox1.Text = ""
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs)
        Dim dialog As New OpenFileDialog()
        If DialogResult.OK = dialog.ShowDialog Then
            local_filename = dialog.FileName
        End If

        ip.Enabled = False
        local = True



    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub
End Class
