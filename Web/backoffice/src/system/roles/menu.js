import React from "react"
import { getTrans } from "../../i18n"

export const Code = "roles"

export const Create = "create"

export const Update = "update"

export const Delete = "delete"

export const Copy = "copy"

export const label = (codes = [], locale = Locales.zh) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return getTrans(
        {
          zh: "新增管理组",
          en: "Add Role"
        },
        locale
      )
    case Update:
      return getTrans(
        {
          zh: "编辑管理组",
          en: "Edit Role"
        },
        locale
      )
    case Delete:
      return getTrans(
        {
          zh: "删除管理组",
          en: "Delete Role"
        },
        locale
      )
    case Copy:
      return getTrans({
        zh: "复制管理组",
        en: "Copy Role"
      })
    default:
      return ""
  }
}

export const codeLabelPair = [Code, ({ codes, locale }) => label(codes, locale)]
