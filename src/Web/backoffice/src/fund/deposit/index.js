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
    FieldWrapper,
    useToasts
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
import clone from "clone-deep"
import { InputField } from "../../input/Input"
import BrandBar from "../../report/BrandBar"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import numberColumn, {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import { DateInputRangeField } from "../../date/DateTimeRangeInput"
import { today } from "../../date/range"
import { useFundDepositFormList } from "./client"
import { Update } from "./menu"
import {
    PaymentMethodLabel,
    PaymentMethodSelect
} from "../../payment/method/PaymentMethod"
import { VerifyFundDepositDisclosure } from "./VerifyFundDeposit"
import { ViewFundDepositDisclosure } from "./ViewFundDeposit"
import FundReviewStatus, {
    FundReviewStatusLabel,
    FundReviewStatusSelect
} from "../FundReviewStatus"
import maxTime from "date-fns/max"
import isEqualTime from "date-fns/isEqual"
import isSameDay from "date-fns/isSameDay"
import isAfter from "date-fns/isAfter"
import parseISO from "date-fns/parseISO"
import isValidTime from "date-fns/isValid"
import { SortOrder } from "../../table/order"
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

const MinuteInMilliseconds = 6000

const BodyContent = ({ brandId }) => {
    let todayRange = today()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        username: "",
        channelName: "",
        method: "",
        status: "",
        beginCreateOn: todayRange.start,
        endCreateOn: todayRange.end,
        beginUpdateOn: undefined,
        endUpdateOn: undefined
    })

    useEffect(() => {
        setState({ pageIndex: 0 })
    }, [brandId])

    let {
        pageIndex,
        pageSize,
        username,
        channelName,
        method,
        status,
        beginCreateOn,
        endCreateOn,
        beginUpdateOn,
        endUpdateOn
    } = state
    let {
        data = {},
        isLoading,
        mutate,
        error,
        deleteCache
    } = useFundDepositFormList(
        brandId,
        {
            ...toApiParams(state),
            bc: state.beginCreateOn,
            ec: startOfNextDay(state.endCreateOn),
            bu: state.beginUpdateOn,
            eu: startOfNextDay(state.endUpdateOn),
            sortby: "CreatedOn",
            orderby: SortOrder.Descending
        }
        /*
        {
            refreshInterval: 1000 //5 * MinuteInMilliseconds //100
        }
        */
    )

    let newOrder = useTrans({
        zh: "新订单",
        en: "new order"
    })
    let multipleOrder = useTrans({
        zh: "New orders",
        en: "Multiple new orders"
    })
    let toasts = useToasts()
    let [items, setItems] = useState([])
    useEffect(() => {
        if (data?.items && !equals(items, data?.items)) {
            let newApplications = newApplicationsFromToday(data.items, items)
            if (newApplications.length) {
                if (newApplications.length < 3) {
                    newApplications.map((application) =>
                        toasts.info({
                            title: `${newOrder}`,
                            message: `${application.userName}`
                        })
                    )
                } else {
                    toasts.info({ title: multipleOrder })
                }
            }
            setItems(clone(data.items))
        }
    }, [JSON.stringify(data?.items)])

    let columns = useColumns({ mutate, items, brandId })
    return useMemo(
        () => (
            <PageContent
                title={<Trans zh="充值列表" en="Deposit List" />}
                actionBar={
                    <SearchBar
                        brandId={brandId}
                        username={username}
                        channelName={channelName}
                        method={method}
                        status={status}
                        beginCreateOn={beginCreateOn}
                        endCreateOn={endCreateOn}
                        beginUpdateOn={beginUpdateOn}
                        endUpdateOn={endUpdateOn}
                        search={(data) => {
                            let newState = {
                                pageIndex: 0,
                                username: data.username,
                                channelName: data.channelName,
                                method: data.method,
                                status: data.status,
                                beginCreateOn: data.beginCreateOn,
                                endCreateOn: data.endCreateOn,
                                beginUpdateOn: data.beginUpdateOn,
                                endUpdateOn: data.endUpdateOn
                            }
                            if (
                                equals(newState, {
                                    pageIndex,
                                    username,
                                    channelName,
                                    method,
                                    status,
                                    beginCreateOn,
                                    endCreateOn,
                                    beginUpdateOn,
                                    endUpdateOn
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
                    data={items}
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
        ),
        [
            brandId,
            JSON.stringify(state),
            isLoading,
            JSON.stringify(items),
            error,
            mutate
        ]
    )
}

const newApplicationsFromToday = (first = [], second = []) => {
    if (!second || second.length === 0) {
        return []
    }

    let today = new Date()
    let lastMaxApplicationTime = maxTime(
        second.reduce((times, { createdOn }) => {
            let value = parseISO(createdOn)
            if (isValidTime(value)) {
                times.push(value)
            }

            return times
        }, [])
    )

    const findSameById = (id, list = []) => list.find((item) => item.id === id)

    let result = first.filter(({ createdOn, id }) => {
        createdOn = parseISO(createdOn)
        return (
            isSameDay(today, createdOn) &&
            isAfter(createdOn, lastMaxApplicationTime) &&
            !findSameById(id, second)
        )
    })

    return result
}

const SearchBar = ({
    brandId,
    username,
    channelName,
    method,
    status,
    beginCreateOn,
    endCreateOn,
    beginUpdateOn,
    endUpdateOn,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            username,
            channelName,
            method,
            status,
            beginCreateOn,
            endCreateOn,
            beginUpdateOn,
            endUpdateOn
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

    let paymentMethodPlaceholder = useTrans({
        zh: "充值类型",
        en: "Deposit Method"
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
                        <PaymentMethodSelect
                            name="method"
                            placeholder={paymentMethodPlaceholder}
                        />
                        <FundReviewStatusSelect name="status" />
                    </Set>
                    <Set orientation="horizontal">
                        <DateInputRangeField
                            label={
                                <Trans zh="申请时间" en="Application Time" />
                            }
                            startProps={{
                                name: "beginCreateOn"
                            }}
                            endProps={{
                                name: "endCreateOn"
                            }}
                        />
                        <DateInputRangeField
                            label={<Trans zh="更新时间" en="Updated Time" />}
                            startProps={{
                                name: "beginUpdateOn"
                            }}
                            endProps={{
                                name: "endUpdateOn"
                            }}
                        />
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
                                    <VerifyFundDepositDisclosure
                                        brandId={brandId}
                                        formId={value.id}
                                        onSubmitted={mutate}
                                    />
                                )}
                                <ViewFundDepositDisclosure
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
