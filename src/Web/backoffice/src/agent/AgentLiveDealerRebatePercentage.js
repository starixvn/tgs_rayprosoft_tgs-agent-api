import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans, Trans } from "../i18n"
import { PercentageNumberInputField } from "../input/NumberInput"

export const Name = "liveDealerRebatePercentage"

const AgentLiveDealerRebateMessages = {
    zh: "真人返佣",
    en: "Live Dealer Rebate"
}

export const AgentLiveDealerRebateText = () => (
    <Trans {...AgentLiveDealerRebateMessages} />
)

export const AgentLiveDealerRebatePercentageInputField = ({
    name = Name,
    required = true,
    ...props
}) => <PercentageNumberInputField name={name} required={required} {...props} />

export default {
    Name
}
