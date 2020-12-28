import React from "react"
import { getTrans } from "../../i18n"

export const Code = "brandproviders"

export const Update = "update"

export const label = ({ codes, locale }) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Update:
      return getTrans({
        zh: "修改",
        en: "Edit"
      })
    default:
      return ""
  }
}

export const codeLabelPair = [Code, label]
