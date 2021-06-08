import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans, Trans } from "../i18n"
import { PercentageNumberInputField } from "../input/NumberInput"

export const Name = "lotteryRebatePercentage"

const AgentLotteryRebateMessages = {
    zh: "彩票返佣",
    en: "Lottery Rebate"
}

export const AgentLotteryRebateText = () => (
    <Trans {...AgentLotteryRebateMessages} />
)

export const AgentLotteryRebatePercentageInputField = ({
    name = Name,
    required = true,
    ...props
}) => <PercentageNumberInputField name={name} required={required} {...props} />

export default {
    Name
}
