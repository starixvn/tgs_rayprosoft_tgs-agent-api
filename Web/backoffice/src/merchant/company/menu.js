import React from "react"
import { Locales, getTrans } from "../../i18n"

export const Code = "company"

export const Update = "update"

export const label = ({ codes = [], locale = Locales.zh }) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Update:
      return getTrans(
        {
          zh: "修改",
          en: "Edit"
        },
        locale
      )
    default:
      return ""
  }
}

export const codeLabelPair = [Code, label]
