import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import client from "../../client"
import PageContent from "../../page/Content"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"

export { Code }

export const Content = () => {
  return <PageContent>TODO: announcement</PageContent>
}
