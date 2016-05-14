<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/Data">
<html xmlns="http://www.w3.org/TR/xhtml1/strict">
  <body>
    <center>
    <h2>ASCII Brightness Data</h2>

    <table border="1" cellpadding="2">
      <tr bgcolor="#9acd32">
        <th>ASCII Code</th>
        <th>ASCII Type</th>
        <th>Font 13 Weight</th>
        <th>Font 16 Weight</th>
        <th>Font <i>xx</i> Weight</th>
        <th>HTML Character</th>
      </tr>

      <xsl:for-each select="ascii">
      <xsl:sort select="id" data-type="number" order="ascending"/>
      <tr align="center">
        <td><xsl:value-of select="id"/></td>

        <xsl:choose>
          <xsl:when test="type = 1">
            <td><font color="#0000ff"><b>Alphabets</b></font></td>
          </xsl:when>
          <xsl:when test="type = 2">
            <td><font color="#ff0000"><b>Numbers</b></font></td>
          </xsl:when>
          <xsl:when test="type = 3">
            <td><font color="#008000"><b>Basic Symbols</b></font></td>
          </xsl:when>
          <xsl:when test="type = 4">
            <td><font color="#800000"><b>Extended Symbols</b></font></td>
          </xsl:when>
          <xsl:when test="type = 5">
            <td><font color="#000080"><b>Block Symbols</b></font></td>
          </xsl:when>
          <xsl:otherwise>
            <td><font color="#000000"><b><i>???</i></b></font></td>
          </xsl:otherwise>
        </xsl:choose>

        <td><xsl:value-of select="c13"/></td>
        <td><xsl:value-of select="c16"/></td>
        <td><xsl:value-of select="c20"/></td>
        <td><xsl:value-of select="htmlchar"/></td>
      </tr>
      </xsl:for-each>
    </table>
    </center>
  </body>
</html>
</xsl:template>

</xsl:stylesheet>
