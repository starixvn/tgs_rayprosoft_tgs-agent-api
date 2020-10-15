import React from "react"
import { getTrans } from "../../i18n"

export const Code = "departments"

export const Create = "create"

export const Update = "update"

export const Delete = "delete"

export const label = (codes = [], locale = Locales.zh) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return getTrans({
        zh: "新增部门",
        en: "Add department"
      }, locale)
    case Update:
      return getTrans({
        zh: "编辑部门",
        en: "Edit department"
      }, locale)
    case Delete:
      return getTrans({
        zh: "删除部门",
        en: "Delete department"
      }, locale)
    default:
      return ""
  }
}

export const codeLabelPair = [Code, ({ codes, locale }) => label(codes, locale)]