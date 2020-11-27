import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { Menu as _Menu, findlLabel } from "../menu"
import {
  Code as CompanyCode,
  codeLabelPair as companyPair
} from "./companies/menu"
import {
  Code as PlatformCode,
  codeLabelPair as platformPair
} from "./platforms/menu"
import { getTrans, useTrans } from "../i18n"

export const nameMessages = {
  zh: "商户管理",
  en: "Merchant Management"
}

export const Name = () => useTrans(nameMessages)

export const Code = "merchant"

export const DefaultUrl = `/${Code}`

export const DefaultPaths = [Code]

export const Menu = ({
  title = <Name />,
  url = DefaultUrl,
  codes,
  paths = DefaultPaths,
  ...props
}) => (
  <_Menu
    title={title}
    url={url}
    codes={codes}
    codePaths={paths}
    itemLabel={label}
  />
)
const label = ({ locale, code }) => {
  switch (code) {
    case CompanyCode:
      return getTrans(
        {
          zh: "公司列表",
          en: "companies"
        },
        locale
      )
    case PlatformCode:
      return getTrans(
        {
          zh: "平台列表",
          en: "platforms"
        },
        locale
      )
    default:
      return ""
  }
}

export const codeLabelPair = [
  Code,
  ({ locale, codes }) =>
    findlLabel({
      locale,
      codes,
      label: label,
      subLabelPairs: [companyPair, platformPair]
    })
]
