/********************************************************************************* 
                         Scorpion

  *FileName:AnswerQuestionFrameCtrler
  *Version:1.0
  *Date:2017-06-06
  *Description:
**********************************************************************************/  
#region using

using System;
using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;
using Random = System.Random;

#endregion

public class AnswerQuestionFrameCtrler : IControllerBase
{

    #region 静态变量
    #endregion

    #region 成员变量

    private int m_iCorrectId = -1; //正确答案index
    private AnswerDataModel DataModel;
    private int m_iGiftCount1; //奖励1需要的正确的答案数
    private int m_iGiftCount2; //奖励2需要的正确的答案数
    private bool m_bIsPlayAnimation; //是否播放动画
    private int MaxCount; //最大可答题数
    private int[] m_arraryRandoms = new int[4]; //随机数组
    private object m_objTimerTrigger;
    private int m_iNextAnswer; //下个答案
    private string m_strQuestion = ""; //第{0}题
    private int m_iRightCount; //答对个数

    #endregion

    #region 构造函数

    public AnswerQuestionFrameCtrler()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_Answer_AnswerClick.EVENT_TYPE, OnQuestionOperateEvent); //答题操作
        EventDispatcher.Instance.AddEventListener(UIEvent_Answer_ExdataUpdate.EVENT_TYPE, OnExGoldUpdateEvent); //扩展数据更新
    }

    #endregion

    #region 固有函数

    public void OnShow()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var _args = data as AnswerArguments;
        if (_args != null)
        {
            if (m_objTimerTrigger != null)
            {
                TimeManager.Instance.DeleteTrigger(m_objTimerTrigger);
                m_objTimerTrigger = null;
            }
            m_strQuestion = GameUtils.GetDictionaryText(270227);
            m_iNextAnswer = PlayerDataManager.Instance.GetExData(436);
            m_iRightCount = PlayerDataManager.Instance.GetExData(437);
            DataModel.AnswerProb = m_iRightCount + "/" + m_iNextAnswer;

            var _beginTime = int.Parse(Table.GetClientConfig(206).Value);
            var _endTime = int.Parse(Table.GetClientConfig(207).Value);
            var _mTimeStart = Game.Instance.ServerTime.AddDays(0).Date.AddHours(_beginTime);
            var _mTimeEnd = Game.Instance.ServerTime.AddDays(0).Date.AddHours(_endTime);
            var _mTimeStart2 = Game.Instance.ServerTime.AddDays(1).Date.AddHours(_beginTime);
            var _mTimeEnd2 = Game.Instance.ServerTime.AddDays(1).Date.AddHours(_endTime);

            var _nowTime = Game.Instance.ServerTime;

            if (m_iNextAnswer >= MaxCount)
            {
                m_iNextAnswer = 0;
                DataModel.IsOver = true;
                DataModel.MTime = _mTimeStart2;
                //活动开启时间
                DataModel.OverString1 = GameUtils.GetDictionaryText(270230);
                //今日活动已完成请明日再来挑战
                DataModel.OverString2 = GameUtils.GetDictionaryText(270228);
                return;
            }

            if (_nowTime < _mTimeStart)
            {
                DataModel.IsOver = true;
                m_iNextAnswer = 0;
                DataModel.MTime = _mTimeStart;
                //活动开启时间
                DataModel.OverString1 = GameUtils.GetDictionaryText(270230);
                //今日活动未开启
                DataModel.OverString2 = GameUtils.GetDictionaryText(270229);
                m_objTimerTrigger = TimeManager.Instance.CreateTrigger(_mTimeStart, () =>
                {
                    TimeManager.Instance.DeleteTrigger(m_objTimerTrigger);
                    m_objTimerTrigger = null;
                    //活动剩余时间
                    DataModel.OverString1 = GameUtils.GetDictionaryText(270231);
                    DataModel.IsOver = false;
                    DataModel.MTime = _mTimeEnd;
                    m_iNextAnswer = 1;
                    if (m_strQuestion != "")
                    {
                        DataModel.WhichQuestion = String.Format(m_strQuestion, m_iNextAnswer);
                        NetManager.Instance.StartCoroutine(GetProblemData(m_iNextAnswer));
                    }
                });
            }
            else if (_nowTime >= _mTimeStart && _nowTime < _mTimeEnd)
            {
                DataModel.IsOver = false;
                //活动剩余时间
                DataModel.OverString1 = GameUtils.GetDictionaryText(270231);
                DataModel.MTime = _mTimeEnd;
                m_objTimerTrigger = TimeManager.Instance.CreateTrigger(_mTimeEnd, () =>
                {
                    TimeManager.Instance.DeleteTrigger(m_objTimerTrigger);
                    m_objTimerTrigger = null;
                    DataModel.IsOver = true;
                    DataModel.MTime = _mTimeStart2;
                    //活动开启时间
                    DataModel.OverString1 = GameUtils.GetDictionaryText(270230);
                    //今日活动已完成请明日再来挑战
                    DataModel.OverString2 = GameUtils.GetDictionaryText(270228);
                });
                m_iNextAnswer++;
                if (m_strQuestion != "")
                {
                    DataModel.WhichQuestion = String.Format(m_strQuestion, m_iNextAnswer);
                    NetManager.Instance.StartCoroutine(GetProblemData(m_iNextAnswer));
                }
            }
            else
            {
                DataModel.IsOver = true;
                m_iNextAnswer = 0;
                DataModel.MTime = _mTimeStart2;
                //今日活动已完成请明日再来挑战
                DataModel.OverString2 = GameUtils.GetDictionaryText(270228);
                //活动开启时间
                DataModel.OverString1 = GameUtils.GetDictionaryText(270230);
                m_objTimerTrigger = TimeManager.Instance.CreateTrigger(_mTimeStart2, () =>
                {
                    TimeManager.Instance.DeleteTrigger(m_objTimerTrigger);
                    m_objTimerTrigger = null;
                    DataModel.IsOver = false;
                    DataModel.MTime = _mTimeEnd2;
                    //活动剩余时间
                    DataModel.OverString1 = GameUtils.GetDictionaryText(270231);
                    m_iNextAnswer = 1;
                    if (m_strQuestion != "")
                    {
                        DataModel.WhichQuestion = String.Format(m_strQuestion, m_iNextAnswer);
                        NetManager.Instance.StartCoroutine(GetProblemData(m_iNextAnswer));
                    }
                });
            }
        }
    }

    public void CleanUp()
    {
        DataModel = new AnswerDataModel();
        DataModel.ItemRight.ItemId = 22051;
        DataModel.ItemRight.Count = 1;
        DataModel.ItemWrong.ItemId = 22052;
        DataModel.ItemWrong.Count = 1;
        MaxCount = int.Parse(Table.GetClientConfig(581).Value);
        var _tbGift1 = Table.GetGift(2000);
        var _tbGift2 = Table.GetGift(2001);
        m_iGiftCount1 = _tbGift1.Param[0];
        m_iGiftCount2 = _tbGift2.Param[0];
        for (var i = 0; i < DataModel.Option.Count; i++)
        {
            DataModel.Option[i] = new AnswerQuestionDataModel();
        }
        for (var i = 0; i < 6; i++)
        {
            var _tt = i % 2;
            if (_tt == 0)
            {
                DataModel.RewardItems8[i / 2] = new ItemIdDataModel();
                DataModel.RewardItems8[i / 2].ItemId = _tbGift1.Param[i + 1];
            }
            else
            {
                DataModel.RewardItems8[i / 2].Count = _tbGift1.Param[i + 1];
            }
        }
        for (var i = 0; i < 6; i++)
        {
            var _tt = i % 2;
            if (_tt == 0)
            {
                DataModel.RewardItems15[i / 2] = new ItemIdDataModel();
                DataModel.RewardItems15[i / 2].ItemId = _tbGift2.Param[i + 1];
            }
            else
            {
                DataModel.RewardItems15[i / 2].Count = _tbGift2.Param[i + 1];
            }
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name.Equals("IsMaxAnser"))
        {
            return (m_iNextAnswer == 0);
        }
        return null;
    }

    public FrameState State { get; set; }

    #endregion

    #region 逻辑函数

    //选项点击事件
    private void AnswerTip(int index)
    {
        if (DataModel.Option[index].TipState != 0)
        {
            return;
        }
        if (index == m_iCorrectId)
        {
            NetManager.Instance.StartCoroutine(SolveProblem(index, 1));
        }
        else
        {
            NetManager.Instance.StartCoroutine(SolveProblem(index, 0));
        }
    }

    //回答问题
    private IEnumerator SolveProblem(int index, int IsRight)
    {
        //using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.AnswerQuestion(IsRight);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    //donghua dengdai
                    {
                        DataModel.Option[index].TipState = 1;
                        if (IsRight == 1)
                        {
                            m_iRightCount++;
                            DataModel.Option[index].State = 1;
                        }
                        else
                        {
                            DataModel.Option[index].State = 2;
                            DataModel.Option[m_iCorrectId].State = 1;
                        }
                        DataModel.AnswerProb = m_iRightCount + "/" + m_iNextAnswer;
                        m_bIsPlayAnimation = true;
                        yield return new WaitForSeconds(1f);
                        m_bIsPlayAnimation = false;
                    }

                    if (m_iNextAnswer >= MaxCount || _msg.Response.QuestionId == -1)
                    {
                        DataModel.IsOver = true;
                        var _mTimeStart2 = Game.Instance.ServerTime.AddDays(1).Date.AddHours(12);
                        DataModel.MTime = _mTimeStart2;
                        if (m_objTimerTrigger != null)
                        {
                            TimeManager.Instance.DeleteTrigger(m_objTimerTrigger);
                            m_objTimerTrigger = null;
                        }
                        //活动开启时间
                        DataModel.OverString1 = GameUtils.GetDictionaryText(270230);
                        //今日活动已完成请明日再来挑战
                        DataModel.OverString2 = GameUtils.GetDictionaryText(270228);

                        yield break;
                    }

                    var tbSubject = Table.GetSubject(_msg.Response.QuestionId);
                    //if (tbSubject != null)
                    //{
                    BuildRadom(_msg.Response);
                    m_iNextAnswer++;
                    DataModel.WhichQuestion = String.Format(m_strQuestion, m_iNextAnswer);

                    // }
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    //直接获得正确答案
    private IEnumerator SolveProblemUseItem()
    {
        //using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.AnswerQuestionUseItem(0);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    //donghua dengdai
                    {
                        DataModel.Option[m_iCorrectId].TipState = 1;
                        DataModel.Option[m_iCorrectId].State = 1;
                        m_iRightCount++;
                        DataModel.AnswerProb = m_iRightCount + "/" + m_iNextAnswer;
                        m_bIsPlayAnimation = true;
                        yield return new WaitForSeconds(1f);
                        m_bIsPlayAnimation = false;
                    }

                    if (m_iNextAnswer >= MaxCount || _msg.Response.QuestionId == -1)
                    {
                        DataModel.IsOver = true;
                        m_iNextAnswer = 0;
                        var _mTimeStart2 = Game.Instance.ServerTime.AddDays(1).Date.AddHours(12);
                        DataModel.MTime = _mTimeStart2;
                        if (m_objTimerTrigger != null)
                        {
                            TimeManager.Instance.DeleteTrigger(m_objTimerTrigger);
                            m_objTimerTrigger = null;
                        }
                        //活动开启时间
                        DataModel.OverString1 = GameUtils.GetDictionaryText(270230);
                        //今日活动已完成请明日再来挑战
                        DataModel.OverString2 = GameUtils.GetDictionaryText(270228);

                        EventDispatcher.Instance.DispatchEvent(new UIEvent_RefreshPush(6, 0));
                        yield break;
                    }

                    //var tbSubject = Table.GetSubject(msg.Response.QuestionId);
                    //if (tbSubject != null)
                    //{
                    BuildRadom(_msg.Response);
                    m_iNextAnswer++;
                    DataModel.WhichQuestion = String.Format(m_strQuestion, m_iNextAnswer);

                    //}
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }


    //获得问题
    private IEnumerator GetProblemData(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.GetQuestionData(index);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    BuildRadom(_msg.Response);
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    //奖励点击事件 显示物品info
    private void OnClickIcon(int row, int index)
    {
        var ItemId = -1;
        if (row == 2)
        {
            ItemId = DataModel.RewardItems8[index].ItemId;
        }
        else if (row == 3)
        {
            ItemId = DataModel.RewardItems15[index].ItemId;
        }

        if (ItemId != -1)
        {
            GameUtils.ShowItemIdTip(ItemId);
        }
    }

    //随机不重复数字排序
    private static int[] StochasticWithoutRepetitionNum(int max, int min)
    {
        var _count = max - min + 1;
        var _array = new int[_count];

        for (var i = 0; i < _count; i++)
        {
            _array[i] = min;
            min++;
        }

        var _random = new Random();
        for (var i = 0; i < _count; i++)
        {
            var _temp = _random.Next(_count);
            var _temp2 = 0;
            _temp2 = _array[_temp];
            _array[_temp] = _array[i];
            _array[i] = _temp2;
        }
        return _array;
    }

    //移除错误答案
    private IEnumerator RemoveMistakeAnswer()
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.RemoveErrorAnswer(0);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    var _mrdm = new Random();
                    var _number = 0;
                    while (true)
                    {
                        _number = _mrdm.Next(0, 4);
                        if (_number != m_iCorrectId)
                        {
                            break;
                        }
                    }

                    DataModel.Option[_number].TipState = 2;
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    //设置问题排列
    private void BuildRadom(QuestionMessage question)
    {
        DataModel.QID = question.QuestionId;
        DataModel.Title = question.Title;
        m_arraryRandoms = StochasticWithoutRepetitionNum(3, 0);
        if (question.Answer.Count != 4)
        {
            Logger.Error(string.Format("question.Answer.Count ={0} "), question.Answer.Count);
            return;
        }
        for (var i = 0; i < m_arraryRandoms.Length; i++)
        {
            if (m_arraryRandoms[i] == 0)
            {
                DataModel.Option[i].Name = question.Answer[0];
                m_iCorrectId = i;
            }
            else
            {
                DataModel.Option[i].Name = question.Answer[m_arraryRandoms[i]];
            }
        }
        for (var i = 0; i < DataModel.Option.Count; i++)
        {
            DataModel.Option[i].State = 0;
            DataModel.Option[i].TipState = 0;
        }
    }

    //帮助按钮事件
    public void HelpButtonClick(int index)
    {
        if (m_iNextAnswer > MaxCount)
        {
            return;
        }
        if (m_iNextAnswer <= 0)
        {
            return;
        }
        switch (index)
        {
            case 0:
                {
                    var _count = PlayerDataManager.Instance.GetItemCount((int)eBagType.BaseItem, 22051);
                    if (_count < 1)
                    {
                        var _e = new ShowUIHintBoard(270002);
                        EventDispatcher.Instance.DispatchEvent(_e);
                        return;
                    }
                    NetManager.Instance.StartCoroutine(SolveProblemUseItem());
                }
                break;
            case 1:
                {
                    var _count = PlayerDataManager.Instance.GetItemCount((int)eBagType.BaseItem, 22052);
                    if (_count < 1)
                    {
                        var _e = new ShowUIHintBoard(270002);
                        EventDispatcher.Instance.DispatchEvent(_e);
                        return;
                    }
                    var _isAlreadyReduce = false;
                    for (var i = 0; i < DataModel.Option.Count; i++)
                    {
                        if (DataModel.Option[i].TipState == 2)
                        {
                            _isAlreadyReduce = true;
                            break;
                        }
                    }
                    if (_isAlreadyReduce)
                    {
                        var _e = new ShowUIHintBoard(270232);
                        EventDispatcher.Instance.DispatchEvent(_e);
                        return;
                    }
                    NetManager.Instance.StartCoroutine(RemoveMistakeAnswer());
                }
                break;
        }
    }

    #endregion

    #region 事件

    //答题操作事件
    private void OnQuestionOperateEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_Answer_AnswerClick;
        if (m_bIsPlayAnimation)
        {
            return;
        }
        switch (_e.Type)
        {
            case 0:
                {
                    AnswerTip(_e.Index);
                }
                break;
            case 1:
                {
                    HelpButtonClick(_e.Index);
                }
                break;
            case 2:
                {
                    OnClickIcon(_e.Type, _e.Index);
                }
                break;
            case 3:
                {
                    OnClickIcon(_e.Type, _e.Index);
                }
                break;
        }
    }
    //答题总经验和金币
    private void OnExGoldUpdateEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_Answer_ExdataUpdate;
        if (_e.Index == (int)eExdataDefine.e66)
        {
            DataModel.GetExp = _e.Value;
        }
        else if (_e.Index == (int)eExdataDefine.e67)
        {
            DataModel.GetGold = _e.Value;
        }
    }


    #endregion 
}