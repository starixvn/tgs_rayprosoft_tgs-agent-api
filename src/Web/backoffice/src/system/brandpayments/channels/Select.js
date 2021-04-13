import React, { useState, useEffect, useMemo, useRef } from "react"
import { usePaymentChannelList } from "./client"
import AlertDialog from "../../../dialog/Alert"
import { Select as _Select } from "../../../select/Select"

export const Select = ({ ...props }) => {
    let { isLoading, data, error } = usePaymentChannelList({})

    console.warn("TODO: map item for i18n?")
    let options = data?.items.map((item) => item) ?? []

    return <_Select isLoading={isLoading} options={options} {...props} />
}

export default _Select
