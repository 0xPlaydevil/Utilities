using UnityEngine;
using System.Collections;
using System;
using System.Data.Common;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;

public class SQLHelper
{
    public bool debug = false;
    /// <summary>
    /// 数据库连接定义
    /// </summary>
    private DbConnection dbConnection;

    /// <summary>
    /// SQL命令定义
    /// </summary>
    private DbCommand dbCommand;

    /// <summary>
    /// 数据读取定义
    /// </summary>
    private DbDataReader dataReader;

    public enum DbType{Sqlite,MySql};

    /// <summary>
    /// 构造函数    
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    public SQLHelper(string connectionString)
    {
        try{
            //构造数据库连接
            dbConnection=new SqliteConnection(connectionString);
            //打开数据库
            dbConnection.Open();
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public SQLHelper(DbType type,string server,string database="",string user="",string pw="",string port="3306")
    {
        try{
			//构造数据库连接
			string conStr;
			switch(type)
			{
				case DbType.Sqlite:
					conStr= "data source="+server;
		            dbConnection=new SqliteConnection(conStr);
					break;
				case DbType.MySql:
					conStr = string.Format("Server={0};Database={1};User ID={2};Password={3};Port={4}",server,database,user,pw,port);
					dbConnection = new MySqlConnection(conStr);
					break;
			}
            //打开数据库
            dbConnection.Open();
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    public DbDataReader ExecuteQuery(string queryString)
    {
        if(debug)   Debug.Log(queryString);
        dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = queryString;
        dataReader = dbCommand.ExecuteReader();
        return dataReader;
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public void CloseConnection()
    {
        //销毁Command
        if(dbCommand != null){
            dbCommand.Cancel();
        }
        dbCommand = null;

        //销毁Reader
        if(dataReader != null){
            dataReader.Close();
        }
        dataReader = null;

        //销毁Connection
        if(dbConnection != null){
            dbConnection.Close();
        }
        dbConnection = null;
    }

    /// <summary>
    /// 读取整张数据表
    /// </summary>
    /// <returns>The full table.</returns>
    /// <param name="tableName">数据表名称</param>
    public DbDataReader ReadFullTable(string tableName,string[] items=null)
    {
        // string queryString = "SELECT * FROM " + tableName;
        if(items==null)
        {
            items= new string[]{"*"};
        }
        string queryString = "SELECT " + items [0];
        for (int i=1; i<items.Length; i++) 
        {
            queryString+=", " + items[i];
        }
        queryString += " FROM " + tableName;
        return ExecuteQuery (queryString);
    }

    /// <summary>
    /// 向指定数据表中插入数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="values">插入的数值</param>
    public DbDataReader InsertValues(string tableName,string[] values)
    {
        //获取数据表中字段数目
        int fieldCount=ReadFullTable(tableName).FieldCount;
        //当插入的数据长度不等于字段数目时引发异常
        if(values.Length!=fieldCount){
            throw new SqliteException("values.Length!=fieldCount");
        }

        string queryString = "INSERT INTO " + tableName + " VALUES (" + values[0];
        for(int i=1; i<values.Length; i++)
        {
            queryString+=", " + values[i];
        }
        queryString += " )";
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 更新指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    /// <param name="key">关键字</param>
    /// <param name="value">关键字对应的值</param>
    public DbDataReader UpdateValues(string tableName,string[] colNames,string[] colValues,string key,string operation,string value)
    {
        //当字段名称和字段数值不对应时引发异常
        if(colNames.Length!=colValues.Length) {
            throw new SqliteException("colNames.Length!=colValues.Length");
        }

        string queryString = "UPDATE " + tableName + " SET " + colNames[0] + "=" + colValues[0];
        for(int i=1; i<colValues.Length; i++) 
        {
            queryString+=", " + colNames[i] + "=" + colValues[i];
        }
        queryString += " WHERE " + key + operation + value;
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public DbDataReader DeleteValuesOR(string tableName,string[] colNames,string[] operations,string[] colValues)
    {
        //当字段名称和字段数值不对应时引发异常
        if(colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length) {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for(int i=1; i<colValues.Length; i++) 
        {
            queryString+="OR " + colNames[i] + operations[0] + colValues[i];
        }
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public DbDataReader DeleteValuesAND(string tableName,string[] colNames,string[] operations,string[] colValues)
    {
        //当字段名称和字段数值不对应时引发异常
        if(colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length) {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for(int i=1; i<colValues.Length; i++) 
        {
            queryString+=" AND " + colNames[i] + operations[i] + colValues[i];
        }
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 创建数据表
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">数据表名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
    public DbDataReader CreateTable(string tableName,string[] colNames,string[] colTypes)
    {
        string queryString = "CREATE TABLE " + tableName + "( " + colNames [0] + " " + colTypes [0];
        for (int i=1; i<colNames.Length; i++) 
        {
            queryString+=", " + colNames[i] + " " + colTypes[i];
        }
        queryString+= "  ) ";
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// Reads the table.
    /// </summary>
    /// <returns>The table.</returns>
    /// <param name="tableName">Table name.</param>
    /// <param name="items">Items.</param>
    /// <param name="colNames">Col names.</param>
    /// <param name="operations">Operations.</param>
    /// <param name="colValues">Col values.</param>
    public DbDataReader ReadTable(string tableName,string[] items,string[] colNames,string[] operations, string[] colValues)
    {
        string queryString = "SELECT " + items [0];
        for (int i=1; i<items.Length; i++) 
        {
            queryString+=", " + items[i];
        }
        queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " +  operations[0] + " " + colValues[0];
        for (int i=1; i<colNames.Length; i++) 
        {
            queryString+=" AND " + colNames[i] + " " + operations[i] + " " + colValues[0] + " ";
        }
        return ExecuteQuery(queryString);
    }
}