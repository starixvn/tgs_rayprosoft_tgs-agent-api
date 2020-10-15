import React from "react"
import { getTrans } from "../../i18n"

export const Code = "admins"

export const Create = "create"

export const Update = "update"

export const label = (codes = [], locale = Locales.zh) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return getTrans({
        zh: "新增管理员",
        en: "Add admin"
      }, locale)
    case Update:
      return getTrans({
        zh: "编辑管理员",
        en: "Edit admin"
      }, locale)
  }
}

export const codeLabelPair = [Code, ({ codes, locale }) => label(codes, locale) ]