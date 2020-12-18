import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, Button, Text, Switch, Input, Stack } from "bumbag"
import PageContent from "../../page/Content"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code, Update as UpdateMenu } from "./menu"
import { useTrans } from "../../i18n"

export { Code }

export const Reseller = "Reseller"

export const ResellerTrans = {
  zh: "总公司",
  en: "Reseller"
}

export const useResellerLabel = () => useTrans(ResellerTrans)

export const Content = () => {
  return ""
}
