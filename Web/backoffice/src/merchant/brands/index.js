import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, Button, Text, Switch, Input, Stack } from "bumbag"
import PageContent from "../../page/Content"
import { useTrans } from "../../i18n"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code, Update as UpdateMenu } from "./menu"

export { Code }

export const Brand = "Brand"

export const BrandTrans = {
  zh: "品牌",
  en: "Brand"
}

export const useBrandLabel = () => useTrans(BrandTrans)

export const Content = () => {
  return ""
}
