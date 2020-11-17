import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Form, Field } from "formik"
import { Trans, useTrans } from "../../i18n"
import { Type as TransferType } from "./transfer"
import { Text } from "bumbag"

// const All = 0
export const All = "All"
const AllTrans = {
  zh: "满足所有",
  en: "All"
}

// const Someone = 1
export const Someone = "Someone"
const SomeoneTrans = {
  zh: "满足其一",
  en: "Satisfy one"
}

const AllPlayers = {
  zh: "所有玩家",
  en: "All Players"
}

export const Detail = ({ record }) => {
  let {
    playerListCondition,
    allPlayer,
    transferType,
    depositedAmountFrom,
    depositedAmountTo,
    transferLimitFrom,
    transferLimitTo,
    identityAuthentication,
    phoneAuthentication,
    vipLevelIds = []
  } = record

  let condition =
    playerListCondition === Someone
      ? useTrans(SomeoneTrans)
      : useTrans(AllTrans)

  let details = [
    allPlayer ? useTrans(AllPlayers) : "",
    transferType === TransferType.Deposit
      ? `${depositedAmountFrom} - ${depositedAmountTo}`
      : `${transferLimitFrom} - ${transferLimitTo}`,
    identityAuthentication
      ? useTrans({ zh: "身份验证", en: "Identity Authentication" })
      : "",
    phoneAuthentication
      ? useTrans({ zh: "手机验证", en: "Phone Authentication" })
      : "",
    useVipLevelDetail(vipLevelIds)
  ].filter((string) => !!string)

  //TODO: one line?
  return (
    <Text.Block whiteSpace="nowrap">{`${condition}: ${details.join(
      ", "
    )}`}</Text.Block>
  )
}

export const Label = ({ value }) => {
  switch (value) {
    case All:
      return <Trans zh="所有玩家" en="All Players" />
    case Someone:
      return <Trans zh="特定玩家" en="Specific Players" />
  }
  return ""
}

export const useVipLevelDetail = (vipLevels = []) => {
  ///TODO: returns vip level detail?
  if (vipLevels.length > 0) {
    return useTrans({
      zh: "VIP群",
      en: "VIP Levels"
    })
  }
  return ""
}
