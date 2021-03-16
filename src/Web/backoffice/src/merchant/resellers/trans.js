import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, Button, Text, Switch, Stack } from "bumbag"
import { Trans, useTrans } from "../../i18n"

const ResellerMessages = {
    zh: "总公司",
    en: "Reseller"
}
export const useResellerTrans = () => useTrans(ResellerMessages)
export const ResellerTrans = () => <Trans {...ResellerMessages} />
