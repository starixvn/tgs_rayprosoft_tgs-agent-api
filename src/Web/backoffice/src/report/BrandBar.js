import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Switch,
    Route,
    Link,
    useLocation,
    useHistory,
    useParams,
    useRouteMatch
} from "react-router-dom"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { Heading, Stack, Box, Button, Set, useModalState } from "bumbag"
import {
    useUserBrandId,
    useUserRoleType,
    useUserResellerId,
    useUserLicenseeId
} from "../user/context"
import { MerchantIdSelect } from "../merchant/MerchantSelect"
import { nextRoleType } from "../system/roles/type"
import equals from "fast-deep-equal"

export const BrandBar = ({ name = "brandBar", onBrandChange, ...props }) => {
    let { url } = useRouteMatch()
    let { location, ...history } = useHistory()

    let userRoleType = useUserRoleType()

    let userResellerId = useUserResellerId()
    let userLicenseeId = useUserLicenseeId()
    let userBrandId = useUserBrandId()

    let [state, setState] = useState({
        resellerId: userResellerId ?? location?.state?.resellerId,
        licenseeId: userLicenseeId ?? location?.state?.licenseeId,
        brandId: userBrandId ?? location?.state?.brandId
    })

    useEffect(() => {
        if (!equals(state, location?.state)) {
            history.replace(url, state)
        }
    }, [JSON.stringify(state), JSON.stringify(location?.state ?? {})])

    useEffect(() => {
        onBrandChange?.(state?.brandId)
    }, [state?.brandId])

    let formMethods = useForm()

    if (userBrandId) {
        return null
    }

    return (
        <FormProvider {...formMethods}>
            <MerchantIdSelect
                start={nextRoleType(userRoleType)}
                resellerProps={{
                    value: state?.resellerId,
                    onChange: (resellerId) => {
                        setState((prevState) => ({
                            ...prevState,
                            resellerId
                        }))
                    }
                }}
                licenseeProps={{
                    value: state?.licenseeId,
                    onChange: (licenseeId) => {
                        setState((prevState) => ({
                            ...prevState,
                            licenseeId
                        }))
                    }
                }}
                brandProps={{
                    value: state?.brandId,
                    onChange: (brandId) => {
                        setState((prevState) => ({
                            ...prevState,
                            brandId
                        }))
                    }
                }}
                paddingTop="1em"
                paddingLeft="1em"
            />
        </FormProvider>
    )
}

export default BrandBar
