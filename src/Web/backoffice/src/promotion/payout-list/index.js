import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Text, Flex } from "bumbag"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserRoleType,
    useUserRoleId,
    useUserBrandId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Code } from "./menu"
import textColumn from "../../table/column/textColumn"
import dateTimeColumn from "../../table/column/dateTime"
import { useForm, FormProvider } from "react-hook-form"
import { InputField } from "../../input/Input"
import SearchButton from "../../button/SearchButton"
import { PageContent } from "../../page/Content"
import AlertDialog from "../../dialog/Alert"
import { FormattedNumberDecimal, Trans, useTrans } from "../../i18n"
import equals from "fast-deep-equal"
import BrandBar from "../../report/BrandBar"
import { formatDateTime, formatDayMonthYear } from "../../date/format"
import { GameTypeLabel } from "../../game/GameType"
import { useGameTypeGroupList } from "../../game/client"
import { formatNumber } from "../../number/format"
import { usePromotionPayoutList } from "./client"
import { PromotionStatusSelect } from "../PromotionStatus"
import { PromotionTypeLabel, usePromotionGroupLabelOf } from "../PromotionType"
import { Checkbox } from "../../checkbox/Checkbox"
import { PromotionManualPayoutDisclosure } from "./ManualPayout"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId }) => {
    // let [payouts, setPayouts] = useState()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        status: "",
        username: "",
        name: "",
        brandId
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let {
        isLoading,
        data,
        error,
        mutate,
        deleteCache
    } = usePromotionPayoutList(toApiParams(state))

    let columns = useColumns({ mutate, brandId, items: data?.items })
    return (
        <PageContent
            title={<Trans zh="活动派发列表" en="Promotion Payouts" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        status={state?.status}
                        username={state?.username}
                        name={state?.name}
                        search={(data) => {
                            let newState = {
                                pageIndex: 0,
                                status: data?.status,
                                username: data?.username,
                                name: data?.name
                            }
                            if (
                                equals(newState, {
                                    pageIndex: state?.pageIndex,
                                    status: state?.status,
                                    username: state?.username,
                                    name: state?.name
                                })
                            ) {
                                mutate()
                            }
                            setState(newState)
                        }}
                        isLoading={isLoading}
                    />
                </Flex>
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(state?.pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({
    status = "",
    username = "",
    name = "",
    search,
    isLoading
}) => {
    let usernamePlaceholder = useTrans({
        zh: "用户名",
        en: "Player Username"
    })
    let namePlaceholder = useTrans({
        zh: "活动名称",
        en: "Promotion Name"
    })
    let formMethods = useForm({
        defaultValues: { status, name, username }
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <Set>
                    <PromotionStatusSelect name="status" />
                    <InputField
                        name="username"
                        showEmptyFieldValueAddonAfter
                        placeholder={usernamePlaceholder}
                    />
                    <InputField
                        name="name"
                        showEmptyFieldValueAddonAfter
                        placeholder={namePlaceholder}
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ brandId, items = [], mutate }) => {
    let hasAnyMenu = useHasSubMenuByRoute()
    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "username",
                Header: <Trans zh="用户名" en="Player Username" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="活动名称" en="Name" />,
                disableSortBy: true
            }),
            textColumn({
                id: "promotionTypeGroup",
                accessor: (row) => row,
                Header: <Trans zh="活动类型" en="Promotion Type" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let group =
                        usePromotionGroupLabelOf(value?.promotionType) ?? ""
                    return group
                }
            }),
            textColumn({
                accessor: "promotionType",
                Header: <Trans zh="活动子类型" en="Promotion Subtype" />,
                disableSortBy: true,
                Cell: ({ value }) => <PromotionTypeLabel value={value} />
            }),
            textColumn({
                id: "promotionTime",
                accessor: (row) => row,
                Header: <Trans zh="生成时间" en="Created Time" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let from = formatDayMonthYear(value?.from)
                    let to = formatDayMonthYear(value?.to)
                    return `${from} - ${to}`
                }
            }),
            textColumn({
                id: "sentTime",
                accessor: (row) => row,
                Header: <Trans zh="派发时间" en="Sent Time" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let from = formatDayMonthYear(value?.from)
                    let to = formatDayMonthYear(value?.to)
                    return `${from} - ${to}`
                }
            }),
            textColumn({
                accessor: "amount",
                Header: <Trans zh="领取金额" en="Payout" />,
                disableSortBy: true,
                Cell: ({ value }) => <FormattedNumberDecimal value={value} />
            }),
            textColumn({
                id: "status",
                Header: <Trans zh="状态" en="Status" />,
                accessor: (row) => row,
                disableSortBy: true,
                Cell: ({ value: record }) => {
                    return "TODO"
                }
            })
        ]

        if (hasAnyMenu) {
            columns.push(
                textColumn({
                    id: "actions",
                    Header: <Trans zh="操作" en="Actions" />,
                    accessor: (row) => row,
                    disableSortBy: true,
                    Cell: ({ value: record }) => {
                        return (
                            <Set orientation="horizontal" alignX="center">
                                <PromotionManualPayoutDisclosure
                                    brandId={brandId}
                                    record={record}
                                    onSubmitted={mutate}
                                />
                            </Set>
                        )
                    }
                })
            )
        }

        return columns
    }, [JSON.stringify(items)])
}
