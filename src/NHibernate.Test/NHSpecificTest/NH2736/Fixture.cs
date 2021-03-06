using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2736
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsVariableLimit;
		}

		protected override void OnSetUp()
		{
			using (var session = Sfi.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				{
					var order = new SalesOrder { Number = 1 };
					order.Items.Add(new Item { SalesOrder = order, Quantity = 1 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 2 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 3 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 4 });
					session.Persist(order);
				}
				{
					var order = new SalesOrder { Number = 2 };
					order.Items.Add(new Item { SalesOrder = order, Quantity = 1 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 2 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 3 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 4 });
					session.Persist(order);
				}
				{
					var order = new SalesOrder { Number = 3 };
					order.Items.Add(new Item { SalesOrder = order, Quantity = 1 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 2 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 3 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 4 });
					session.Persist(order);
				}
				{
					var order = new SalesOrder { Number = 4 };
					order.Items.Add(new Item { SalesOrder = order, Quantity = 1 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 2 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 3 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 4 });
					session.Persist(order);
				}
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = Sfi.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Item").ExecuteUpdate();
				session.CreateQuery("delete from SalesOrder").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void TestHqlParametersWithTake()
		{
			using (var session = Sfi.OpenSession())
			using (session.BeginTransaction())
			{
				var query = session.CreateQuery("select o.Id, i.Id from SalesOrder o left join o.Items i with i.Quantity = :pQuantity take :pTake");
				query.SetParameter("pQuantity", 1);
				query.SetParameter("pTake", 2);
				var result = query.List();
				Assert.That(result.Count, Is.EqualTo(2));
			}
		}
	}
}
