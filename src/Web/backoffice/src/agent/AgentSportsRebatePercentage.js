import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans, Trans } from "../i18n"
import { PercentageNumberInputField } from "../input/NumberInput"

export const Name = "sportsRebatePercentage"

const AgentSportsRebateMessages = {
    zh: "体育返佣",
    en: "Sports Rebate"
}

export const AgentSportsRebateText = () => (
    <Trans {...AgentSportsRebateMessages} />
)

export const AgentSportsRebatePercentageInputField = ({
    name = Name,
    required = true,
    ...props
}) => <PercentageNumberInputField name={name} required={required} {...props} />

export default {
    Name
}
