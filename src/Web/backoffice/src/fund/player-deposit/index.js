import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Box,
    Button,
    Set,
    useModalState,
    Flex,
    Level,
    FieldStack,
    FieldWrapper
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code } from "./menu"
import { FormattedNumberDecimal, Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import BrandBar from "../../report/BrandBar"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import numberColumn, {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import { today } from "../../date/range"
import { usePlayerDepositFormList } from "./client"
import { Update } from "./menu"
import {
    PaymentMethodLabel,
    PaymentMethodSelect
} from "../../payment/method/PaymentMethod"
import { VerifyPlayerDepositDisclosure } from "./VerifyPlayerDeposit"
import { ViewPlayerDepositDisclosure } from "./ViewPlayerDeposit"
import { mutate } from "swr"
import FundReviewStatus, { FundReviewStatusLabel } from "../FundReviewStatus"

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
    let todayRange = today()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        username: "",
        channelName: "",
        paymentChannel: "",
        status: "",
        createdOnStart: todayRange.start,
        createdOnEnd: todayRange.end,
        updatedOnStart: undefined,
        updatedOnEnd: undefined
    })

    useEffect(() => {
        setState({ pageIndex: 0 })
    }, [brandId])

    let {
        pageIndex,
        pageSize,
        username,
        channelName,
        paymentChannel,
        status,
        createdOnStart,
        createdOnEnd,
        updatedOnStart,
        updatedOnEnd
    } = state
    let {
        data = {},
        isLoading,
        mutate,
        error,
        deleteCache
    } = usePlayerDepositFormList(brandId, {
        ...toApiParams(state)
        // sortby: "CreatedOn" //TODO: rm when bug fixed
    })

    let columns = useColumns({ mutate, items: data?.items, brandId })

    return (
        <PageContent
            title={<Trans zh="充值列表" en="Deposit List" />}
            actionBar={
                <SearchBar
                    brandId={brandId}
                    username={username}
                    channelName={channelName}
                    paymentChannel={paymentChannel}
                    status={status}
                    createdOnStart={createdOnStart}
                    createdOnEnd={createdOnEnd}
                    updatedOnStart={updatedOnStart}
                    updatedOnEnd={updatedOnEnd}
                    search={(data) => {
                        let newState = {
                            pageIndex: 0,
                            username: data.username,
                            channelName: data.channelName,
                            paymentChannel: data.paymentChannel,
                            status: data.status,
                            createdOnStart: data.createdOnStart,
                            createdOnEnd: data.createdOnEnd,
                            updatedOnStart: data.updatedOnStart,
                            updatedOnEnd: data.updatedOnEnd
                        }
                        if (
                            equals(newState, {
                                pageIndex,
                                username,
                                channelName,
                                paymentChannel,
                                status,
                                createdOnStart,
                                createdOnEnd,
                                updatedOnStart,
                                updatedOnEnd
                            })
                        ) {
                            mutate()
                        }
                        setState({
                            pageIndex: 0,
                            ...data
                        })
                    }}
                    isLoading={isLoading}
                />
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
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
    brandId,
    username,
    channelName,
    paymentChannel,
    status,
    createdOnStart,
    createdOnEnd,
    updatedOnStart,
    updatedOnEnd,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            username,
            channelName,
            paymentChannel,
            status,
            createdOnStart,
            createdOnEnd,
            updatedOnStart,
            updatedOnEnd
        }
    })

    let usernameInputPlaceholder = useTrans({
        zh: "用户名",
        en: "Username"
    })

    let channelNameInputPlaceholder = useTrans({
        zh: "通道名称",
        en: "Channel Name"
    })

    let createdOnLabel = useTrans({
        zh: "申请时间",
        en: "Application Time"
    })
    let updatedOnLabel = useTrans({
        zh: "更新时间",
        en: "Update time"
    })
    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <FieldStack orientation="vertical">
                    <Set orientation="horizontal">
                        <InputField
                            name="username"
                            showEmptyFieldValueAddonAfter
                            placeholder={usernameInputPlaceholder}
                        />
                        <InputField
                            name="channelName"
                            showEmptyFieldValueAddonAfter
                            placeholder={channelNameInputPlaceholder}
                        />
                        <PaymentMethodSelect name="method" />
                    </Set>
                    <Set orientation="horizontal">
                        <FieldWrapper label={createdOnLabel}>
                            <DateInputRange
                                startProps={{
                                    name: "createdOnStart"
                                }}
                                endProps={{
                                    name: "createdOnEnd"
                                }}
                            />
                        </FieldWrapper>

                        <FieldWrapper label={updatedOnLabel}>
                            <DateInputRange
                                startProps={{
                                    name: "updatedOnStart"
                                }}
                                endProps={{
                                    name: "updatedOnEnd"
                                }}
                            />
                        </FieldWrapper>
                        <SearchButton
                            isLoading={isLoading}
                            disabled={isLoading}
                            alignSelf="flex-end"
                        />
                    </Set>
                </FieldStack>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ brandId, items = [], mutate }) => {
    let hasUpdateMenu = useIsDisplayMenuByRoute([Update])

    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            textColumn({
                accessor: "userName",
                Header: <Trans zh="用户名" en="Username" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "id",
                Header: <Trans zh="订单号" en="Id" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            textColumn({
                id: "paymentMethod",
                accessor: (row) => row,
                Header: <Trans zh="充值类型" en="Payment Method" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    return (
                        <Box>
                            <PaymentMethodLabel
                                value={value?.paymentMethodId}
                            />
                        </Box>
                    )
                }
            }),
            textColumn({
                accessor: "channelName",
                Header: <Trans zh="通道名称" en="Payment Name" />,
                disableSortBy: true,
                Cell: ({ value }) => <Box>{value}</Box>
            }),
            numberDecimalColumn({
                accessor: "amount",
                Header: <Trans zh="充值金额" en="Amount" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "createdOn",
                Header: <Trans zh="申请时间" en="Application Time" />,
                disableSortBy: true
            }),

            dateTimeColumn({
                accessor: "updatedOn",
                Header: <Trans zh="更新时间" en="Update Time" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "status",
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: ({ value }) => <FundReviewStatusLabel value={value} />
            }),
            textColumn({
                accessor: "remark",
                Header: <Trans zh="备注" en="Remark" />,
                disableSortBy: true,
                Cell: ({ value }) => <Box>{value}</Box>
            })
        ]

        if (hasUpdateMenu) {
            columns.push(
                textColumn({
                    id: "actions",
                    accessor: (row) => row,
                    disableSortBy: true,
                    Header: <Trans zh="操作" en="Actions" />,
                    Cell: ({ value }) => {
                        let underReview =
                            value?.status === FundReviewStatus.Review
                        return (
                            <Set alignX="center">
                                {underReview && (
                                    <VerifyPlayerDepositDisclosure
                                        brandId={brandId}
                                        formId={value.id}
                                        onSubmitted={mutate}
                                    />
                                )}
                                <ViewPlayerDepositDisclosure
                                    brandId={brandId}
                                    formId={value.id}
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
