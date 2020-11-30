import React, { useState, useEffect, useMemo, useRef } from "react"
import { Locales, Trans, useTrans } from "../../i18n"

export const InVerify = 0
export const InVerifyName = "InVerify"
const InVerifyTrans = {
  zh: "审核中",
  en: "Under review"
}

export const Published = 1
export const PublishedName = "Published"
const PublishedTrans = {
  zh: "已发布",
  en: "Published"
}

export const Rejected = 2
export const RejectedName = "Rejected"
const RejectedTrans = {
  zh: "已拒绝",
  en: "Rejected"
}

//string -> number
export const from = (value) => {
  switch (value) {
    case InVerifyName:
    case `${InVerify}`:
      return InVerify

    case PublishedName:
    case `${Published}`:
      return Published

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

    case Published:
    case PublishedName:
      return <Trans {...PublishedTrans} />

    case Rejected:
    case RejectedName:
      return <Trans {...RejectedTrans} />

    default:
      return value
  }
}
