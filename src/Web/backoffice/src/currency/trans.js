import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Trans, useTrans } from "../i18n"

const CurrencyMessages = {
    zh: "币种",
    en: "Currency"
}
export const useCurrencyTrans = () => useTrans(CurrencyMessages)
export const CurrencyTrans = () => <Trans {...CurrencyMessages} />
