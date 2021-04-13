import React, { useState, useEffect, useMemo, useRef } from "react"
import { usePaymentList } from "./client"
import { Select as _Select } from "../../select/Select"

export const Select = ({ ...props }) => {
    let { isLoading, data, error } = usePaymentList()

    console.warn("TODO: map item for i18n?")
    let options = data?.items.map((item) => item) ?? []

    return <_Select isLoading={isLoading} options={options} {...props} />
}

export default Select
