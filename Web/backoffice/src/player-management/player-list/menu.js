import React from "react"
import { Locales, getTrans } from "../../i18n"

export const Code = "playerlist"

export const View = "view"

export const Update = "update"

export const codeLabelPair = [Code, ({ codes, locale }) => label(codes, locale)]

export const label = (codes = [], locale = Locales.zh) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case View:
      return getTrans(
        {
          zh: "查看玩家",
          en: "View player"
        },
        locale
      )
    case Update:
      return getTrans(
        {
          zh: "编辑玩家",
          en: "Edit player"
        },
        locale
      )
    default:
      return ""
  }
}
