import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, Button, Text, Switch, Stack } from "bumbag"
import { Trans, useTrans } from "../../i18n"

const LicenseeMessages = {
    zh: "公司",
    en: "Licensee"
}
export const useLicenseeTrans = () => useTrans(LicenseeMessages)
export const LicenseeTrans = () => <Trans {...LicenseeMessages} />
