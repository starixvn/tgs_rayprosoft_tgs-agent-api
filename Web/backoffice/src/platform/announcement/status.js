import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Locales, Trans, useTrans } from "../../i18n"

export const InVerify = 0
export const InVerifyName = "InVerify"
const InVerifyTrans = {
  zh: "审核中",
  en: "Under review"
}

export const Effective = 1
export const EffectiveName = "Effective"
const EffectiveTrans = {
  zh: "有效",
  en: "Effective"
}

export const Invalid = 2
export const InvalidName = "Invalid"
const InvalidTrans = {
  zh: "无效",
  en: "Invalid"
}

export const Expired = 3
export const ExpiredName = "Expired"
const ExpiredTrans = {
  zh: "过期",
  en: "Expired"
}

export const Rejected = 4
export const RejectedName = "Rejected"
const RejectedTrans = {
  zh: "过期",
  en: "Expired"
}

//string -> number
export const from = (value) => {
  switch (value) {
    case InVerifyName:
    case `${InVerify}`:
      return InVerify

    case EffectiveName:
    case `${Effective}`:
      return Effective

    case InvalidName:
    case `${Invalid}`:
      return Invalid

    case ExpiredName:
    case `${Expired}`:
      return Expired

    case RejectedName:
    case `${Rejected}`:
      return Rejected

    default:
      return value
  }
}

export const Label = ({ value }) => {
  switch (value) {
    case InVerify:
    case InVerifyName:
      return <Trans {...InVerifyTrans} />

    case Effective:
    case EffectiveName:
      return <Trans {...EffectiveTrans} />

    case Invalid:
    case InvalidName:
      return <Trans {...InvalidTrans} />

    case Expired:
    case ExpiredName:
      return <Trans {...ExpiredTrans} />

    case Rejected:
    case RejectedName:
      return <Trans {...RejectedTrans} />

    default:
      return value
  }
}
