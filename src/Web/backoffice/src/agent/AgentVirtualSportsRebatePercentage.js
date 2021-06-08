import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans, Trans } from "../i18n"
import { PercentageNumberInputField } from "../input/NumberInput"

export const Name = "virtualSportsRebatePercentage"

const AgentVirtualSportsRebateMessages = {
    zh: "虚拟体育返佣",
    en: "Virtual Sports Rebate"
}

export const AgentVirtualSportsRebateText = () => (
    <Trans {...AgentVirtualSportsRebateMessages} />
)

export const AgentVirtualSportsRebatePercentageInputField = ({
    name = Name,
    required = true,
    ...props
}) => <PercentageNumberInputField name={name} required={required} {...props} />

export default {
    Name
}
