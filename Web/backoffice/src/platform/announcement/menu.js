import React from "react"
import { Locales, getTrans } from "../../i18n"

export const Code = "announcement"

export const Create = "create"

export const Update = "update"

export const Audit = "audit"

export const View = "view"

export const label = ({ codes = [], locale = Locales.zh }) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return getTrans(
        {
          zh: "新增公告",
          en: "Add announcement"
        },
        locale
      )
    case Update:
      return getTrans(
        {
          zh: "修改公告",
          en: "Edit announcement"
        },
        locale
      )
    case Audit:
      return getTrans(
        {
          zh: "审核公告",
          en: "Audit announcement"
        },
        locale
      )
    case View:
      return getTrans(
        {
          zh: "查看公告",
          en: "View announcement"
        },
        locale
      )
    default:
      return ""
  }
}

export const codeLabelPair = [Code, label]
