using System.Text;

namespace ArchitectSample.Physical
{
    internal class SqlBuilder
    {
        private readonly StringBuilder builder;

        public SqlBuilder()
        {
            this.builder = new StringBuilder();
        }

        public static SqlBuilder operator +(SqlBuilder sqlBuilder, string sql)
        {
            sqlBuilder.Append(sql);

            return sqlBuilder;
        }

        public static implicit operator SqlBuilder(string sql)
        {
            var sqlBuilder = new SqlBuilder();

            sqlBuilder.Append(sql);

            return sqlBuilder;
        }

        public static implicit operator string(SqlBuilder sqlBuilder)
        {
            return sqlBuilder.ToString();
        }

        public void Append(string sql)
        {
            this.builder.Append(sql);
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}