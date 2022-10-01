using System;
using System.Data;
using System.Xml.Serialization;
using System.IO;

/// <summary>
/// Represents errors that occur during Login with Web App Shield
/// </summary>
[Serializable()]
public class WASException : Exception
{
	private int m_errorCode = 0;

	[XmlElementAttribute("ErrorCode", typeof(Int32))]
	public int ErrorCode
	{
		get { return m_errorCode; }
		set { m_errorCode = value; }
	}

	public WASException()
	{

	}

	public WASException(int pErrorCode, string pMessage)
		: base(pMessage)
	{
		m_errorCode = pErrorCode;
	}

	public WASException(int pErrorCode, string pMessage, Exception pInner)
		: base(pMessage, pInner)
	{
		m_errorCode = pErrorCode;
	}

	[XmlIgnoreAttribute()]
	public DataTable ReturnValue
	{
		get { return buildDataTable(); }
	}

	private DataTable buildDataTable()
	{
		DataTable dtReturn = new DataTable("WASException");

		DataRow dr = null;
		dtReturn.Columns.Add(new DataColumn("Index", typeof(int)));
		dtReturn.Columns.Add(new DataColumn("ErrorType", typeof(string)));
		dtReturn.Columns.Add(new DataColumn("ErrorNumber", typeof(int)));
		dtReturn.Columns.Add(new DataColumn("ErrorDescription", typeof(string)));
		dr = dtReturn.NewRow();
		dr["Index"] = 0;
		dr["ErrorType"] = "WASException";
		dr["ErrorNumber"] = m_errorCode;
		dr["ErrorDescription"] = this.Message;
		dtReturn.Rows.Add(dr);

		Exception ex = this.InnerException;
		int iIndex = 1;

		while (ex != null)
		{
			if (ex.GetType().ToString() == this.GetType().ToString())
			{
				dr = dtReturn.NewRow();
				dr["Index"] = iIndex;
				dr["ErrorType"] = ex.GetType().ToString();
				dr["ErrorNumber"] = (ex as WASException).m_errorCode;
				dr["ErrorDescription"] = ex.Message;
				dtReturn.Rows.Add(dr);
			}
			else
			{
				dr = dtReturn.NewRow();
				dr["Index"] = iIndex;
				dr["ErrorType"] = ex.GetType().ToString();
				dr["ErrorNumber"] = 0;
				dr["ErrorDescription"] = ex.Message;
				dtReturn.Rows.Add(dr);
			}
			iIndex++;
			ex = ex.InnerException;
		}

		return dtReturn;
	}

}
