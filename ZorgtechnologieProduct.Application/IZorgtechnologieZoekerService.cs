
using System;
using System.Collections.Generic;

namespace ZorgtechnologieProduct.Application
{
    public interface IZorgtechnologieZoekerService
    {
        List<Domain.ZorgtechnologieProduct> FilterOpCriteria(ZorgtechnologieFilter filter);
        bool CheckBeschikbaarheid(Guid id);
        Domain.ZorgtechnologieProduct GeefDetails(Guid id);
    }
}
