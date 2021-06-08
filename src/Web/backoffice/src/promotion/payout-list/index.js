import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Text, Flex, Dialog, Stack } from "bumbag"
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
import {
    FormatHourMinuteSecondDayMonthYear,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import equals from "fast-deep-equal"
import BrandBar from "../../report/BrandBar"
import { formatDateTime, formatDayMonthYear } from "../../date/format"
import { GameTypeLabel } from "../../game/GameType"
import { useGameTypeGroupList } from "../../game/client"
import { formatNumber } from "../../number/format"
import { usePromotionPayoutList, useSendPromotionPayout } from "./client"
import {
    PromotionTypeGroupSelect,
    PromotionTypeLabel,
    usePromotionGroupLabelOf
} from "../PromotionType"
import { Checkbox } from "../../checkbox/Checkbox"
import {
    PromotionManualPayoutDisclosure,
    ViewPromotionManualPayoutDisclosure
} from "./ManualPayout"
import { PayoutStatusLabel } from "./PayoutStatus"
import clone from "clone-deep"
import { DateInputRangeField } from "../../date/DateTimeRangeInput"
import { today } from "../../date/range"
import { startOfNextDay } from "../../date/startOfNextDay"

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

const includePayout = (record = {}, list = []) => {
    return !!list.find(({ id }) => id === record?.id)
}

const BodyContent = ({ brandId }) => {
    let todayRange = today()
    let [batchPayouts, setBatchPayouts] = useState([])

    const appendBatchPayout = (record) => {
        if (!includePayout(record, batchPayouts)) {
            setBatchPayouts((batchPayouts = []) => {
                let result = [...batchPayouts, record]
                console.log("setBatchPayouts result", result)
                return result
            })
        }
    }

    const dropBatchPayout = (record) => {
        let payouts = batchPayouts.filter((payout) => payout.id != record?.id)
        setBatchPayouts(payouts)
    }

    const resetBatchPayout = () => setBatchPayouts([])

    let { state, setState, setPageIndex, setSortOrder } = useTable({
        promotionGroup: "",
        username: "",
        name: "",
        start: undefined,
        end: undefined,
        brandId
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let {
        isLoading: isLoadingPromotionList,
        data,
        error: promotionListError,
        mutate,
        deleteCache
    } = usePromotionPayoutList({
        ...toApiParams(state),
        end: startOfNextDay(state.end)
    })

    let [formData, setSubmit] = useState()
    let {
        isLoading: isLoadingSendPromotionPayout,
        result,
        error: sendPromotionPayoutError,
        mutate: mutateSendPromotionPayout,
        deleteCache: deleteSendPromotionPayoutCache
    } = useSendPromotionPayout(formData)

    let isLoading = isLoadingPromotionList || isLoadingSendPromotionPayout
    let error = promotionListError || sendPromotionPayoutError

    useEffect(() => {
        if (result && !sendPromotionPayoutError) {
            resetBatchPayout()
            mutate()
        }
    }, [result])

    let columns = useColumns({
        mutate,
        brandId,
        items: data?.items,
        batchPayouts,
        setBatchPayouts,
        appendBatchPayout,
        dropBatchPayout,
        resetBatchPayout
    })
    return (
        <PageContent
            title={<Trans zh="活动派发列表" en="Promotion Payouts" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        promotionGroup={state?.promotionGroup}
                        username={state?.username}
                        name={state?.name}
                        start={state?.start}
                        end={state?.end}
                        search={(data = {}) => {
                            setState({
                                pageIndex: 0,
                                ...data
                            })
                            mutate()
                        }}
                        isLoading={isLoading}
                    />
                </Flex>
            }
        >
            <Stack>
                <Table
                    controlledState={state}
                    columns={columns}
                    data={data?.items}
                    pageCount={pageCount(state?.pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    isLoading={isLoading}
                />
                <Dialog.Footer position="sticky" bottom="0">
                    <Button
                        isLoading={isLoadingSendPromotionPayout}
                        onClick={() => {
                            let players = batchPayouts.map(
                                ({
                                    promotionBonusId,
                                    playerCode,
                                    ...record
                                }) => {
                                    return {
                                        promotionBonusId,
                                        playerCode
                                    }
                                }
                            )
                            setSubmit({ players })
                        }}
                    >
                        <Trans zh="批量派发" en="Batch Payout" />
                    </Button>
                </Dialog.Footer>
            </Stack>

            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        if (sendPromotionPayoutError) {
                            resetBatchPayout()
                            deleteSendPromotionPayoutCache()
                            mutateSendPromotionPayout(
                                { data: undefined },
                                false
                            )
                        } else if (promotionListError) {
                            deleteCache()
                            mutate({ data: undefined }, false)
                        }
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({
    promotionGroup = "",
    username = "",
    name = "",
    start,
    end,
    search,
    isLoading
}) => {
    let usernamePlaceholder = useTrans({
        zh: "用户名",
        en: "Player Username"
    })
    let namePlaceholder = useTrans({
        zh: "活动名",
        en: "Promotion Name"
    })
    let formMethods = useForm({
        defaultValues: { promotionGroup, name, username, start, end }
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <Set alignY="bottom">
                    <PromotionTypeGroupSelect name="promotionGroup" />
                    <InputField
                        name="name"
                        showEmptyFieldValueAddonAfter
                        placeholder={namePlaceholder}
                    />
                    <InputField
                        name="username"
                        showEmptyFieldValueAddonAfter
                        placeholder={usernamePlaceholder}
                    />
                    <DateInputRangeField
                        label={<Trans zh="派发时间" en="Sent On" />}
                        startProps={{
                            name: "start"
                        }}
                        endProps={{
                            name: "end"
                        }}
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({
    brandId,
    items = [],
    mutate,
    batchPayouts = [],
    setBatchPayouts,
    appendBatchPayout,
    dropBatchPayout,
    resetBatchPayout
}) => {
    let hasAnyMenu = useHasSubMenuByRoute()
    return useMemo(() => {
        let columns = [
            textColumn({
                id: "batchPayout",
                accessor: (row) => row,
                Header: () => {
                    let isAllChecked =
                        items.length > 0 && items.length === batchPayouts.length
                    return (
                        <Checkbox
                            checked={isAllChecked}
                            onChange={(event) => {
                                let checked = event.target.checked
                                if (checked) {
                                    let records = items.map((promotion) =>
                                        clone(promotion)
                                    )
                                    setBatchPayouts(records)
                                } else {
                                    resetBatchPayout()
                                }
                            }}
                        />
                    )
                },
                disableSortBy: true,
                Cell: ({ value: record }) => {
                    let checked = includePayout(record, batchPayouts)
                    console.log(
                        "checked",
                        checked,
                        "record",
                        record,
                        "batchPayouts",
                        batchPayouts
                    )
                    return (
                        <Checkbox
                            checked={checked}
                            onChange={(event) => {
                                let checked = event.target.checked
                                if (checked) {
                                    appendBatchPayout(record)
                                } else {
                                    dropBatchPayout(record)
                                }
                            }}
                        />
                    )
                }
            }),
            textColumn({
                accessor: "username",
                Header: <Trans zh="用户名" en="Username" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "promotionName",
                Header: <Trans zh="活动名称" en="Promotion Name " />,
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
                id: "createdOn",
                accessor: "createdOn",
                Header: <Trans zh="生成时间" en="Created Time" />,
                disableSortBy: true,
                Cell: ({ value }) => (
                    <FormatHourMinuteSecondDayMonthYear
                        value={value}
                        placeholder="-"
                    />
                )
            }),
            textColumn({
                accessor: "sentOn",
                Header: <Trans zh="派发时间" en="Sent On" />,
                disableSortBy: true,
                Cell: ({ value }) => (
                    <FormatHourMinuteSecondDayMonthYear
                        value={value}
                        placeholder="-"
                    />
                )
            }),
            textColumn({
                accessor: "bonusAmount",
                Header: <Trans zh="领取金额" en="Payout" />,
                disableSortBy: true,
                Cell: ({ value }) => <FormattedNumberDecimal value={value} />
            }),
            textColumn({
                id: "status",
                accessor: (row) => row,
                Header: <Trans zh="状态" en="Status" />,
                accessor: (row) => row,
                disableSortBy: true,
                Cell: ({ value: record }) => {
                    return <PayoutStatusLabel value={record?.sentOn} />
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
                                <ViewPromotionManualPayoutDisclosure
                                    brandId={brandId}
                                    record={record}
                                />
                            </Set>
                        )
                    }
                })
            )
        }

        return columns
    }, [JSON.stringify(items), JSON.stringify(batchPayouts)])
}
