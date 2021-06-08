import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans, Trans } from "../i18n"
import { PercentageNumberInputField } from "../input/NumberInput"

export const Name = "cardGameRebatePercentage"

const AgentCardGameRebateMessages = {
    zh: "棋牌返佣",
    en: "Card Game Rebate"
}

export const AgentCardGameRebateText = () => (
    <Trans {...AgentCardGameRebateMessages} />
)

export const AgentCardGameRebatePercentageInputField = ({
    name = Name,
    required = true,
    ...props
}) => <PercentageNumberInputField name={name} required={required} {...props} />

export default {
    Name
}
