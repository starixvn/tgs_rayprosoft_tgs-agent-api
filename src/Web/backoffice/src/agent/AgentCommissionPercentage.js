import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans, Trans } from "../i18n"
import { PercentageNumberInputField } from "../input/NumberInput"

export const Name = "commissionPercentage"

export const AgentCommissionPercentageInputField = ({
    name = Name,
    required = true,
    ...props
}) => <PercentageNumberInputField name={name} required={required} {...props} />

export default {
    Name
}
