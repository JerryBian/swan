using GitStoreDotnet;
using Swan.Core.Model;

namespace Swan.Core.Store
{
    public class SwanStore
    {
        private readonly IGitStore _gitStore;
        private SwanObject _swanObject;

        public SwanStore(IGitStore gitStore)
        {
            _gitStore = gitStore;
        }

        public async Task LoadAsync()
        {
            //_gitStore.
        }
    }
}
