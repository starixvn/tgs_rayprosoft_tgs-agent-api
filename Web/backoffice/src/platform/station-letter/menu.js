import React from "react"
import { Locales, getTrans } from "../../i18n"

export const Code = "stationletter"

export const Create = "create"

export const Audit = "audit"

export const View = "view"

export const label = ({ codes = [], locale = Locales.zh }) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return getTrans(
        {
          zh: "新增站内信",
          en: "Add station letter"
        },
        locale
      )
    case Audit:
      return getTrans(
        {
          zh: "审核站内信",
          en: "Audit station letter"
        },
        locale
      )
    case View:
      return getTrans(
        {
          zh: "查看站内信",
          en: "View station letter"
        },
        locale
      )
    default:
      return ""
  }
}

export const codeLabelPair = [Code, label]
