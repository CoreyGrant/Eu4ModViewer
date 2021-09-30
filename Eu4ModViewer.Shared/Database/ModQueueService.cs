using Eu4ModViewer.Shared.Entities;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eu4ModViewer.Shared.Database
{
	public interface IModQueueService
	{
		List<ModQueue> GetPending();
		void MarkAsFailed(long modId);
		void MarkAsSuccess(long modId);
		int AddToQueue(ModQueue modQueue);
		List<ModQueue> GetLatest(int amount);
		bool ModIsQueued(long modId);
	}

	public class ModQueueService : IModQueueService
	{
		private readonly Eu4ModViewerDatabase _database;

		public ModQueueService(Eu4ModViewerDatabase database)
		{
			_database = database;
		}

		public List<ModQueue> GetLatest(int amount)
		{
			return this._database.ModQueue.OrderByDescending(x => x.ModQueueId).DistinctBy(x => x.ModId).Take(amount).ToList();
		}

		public int AddToQueue(ModQueue modQueue)
		{
			var result = this._database.ModQueue.Add(modQueue).Entity.ModQueueId;
			_database.SaveChanges();
			return result;
		}

		public bool ModIsQueued(long modId)
		{
			return this._database.ModQueue.Any(x => x.ModId == modId && x.Status != 3);
		}

		public List<ModQueue> GetPending()
		{
			var modQueues = _database.ModQueue.Where(x => x.Status == 0).ToList();
			foreach (var mq in modQueues)
			{
				mq.Status = 1;
				_database.ModQueue.Update(mq);
				_database.SaveChanges();
			}
			return modQueues;
		}

		public void MarkAsSuccess(long modId)
		{
			var mq = _database.ModQueue.SingleOrDefault(x => x.ModId == modId && x.Status == 1);
			if (mq == null) { return; }
			mq.Status = 2;
			_database.ModQueue.Update(mq);
			_database.SaveChanges();
		}

		public void MarkAsFailed(long modId)
		{
			var mq = _database.ModQueue.SingleOrDefault(x => x.ModId == modId && x.Status == 1);
			if(mq == null) { return; }
			mq.Status = 3;
			_database.ModQueue.Update(mq);
			_database.SaveChanges();
		}
	}
}
